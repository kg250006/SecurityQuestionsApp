using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog.Web;
using Newtonsoft.Json.Linq;


namespace DBService.DB.Models
{
    public class SQLConfigRepository : IConfigRepository
    {
        private readonly IContextRepository context;
        private NLog.Logger _log = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public SQLConfigRepository(IContextRepository context) {
            this.context = context;
        }

        public Config GetById(int id)
        {
            return context.Configs.Find(id);
        }

        public string GetValue(string name)
        {
            var data = GetAll().FirstOrDefault(d => d.Name == name);
            if(data != null)
            {
                return data.Value;
            }
            else
                return string.Empty;
        }

        public IEnumerable<Config> GetAll()
        {
            return context.Configs;
        }

        public List<Config> GetByTimestamp(DateTime dfrom, DateTime dTo)
        {
            string sFunc = "GetByTimestamp - ";
            var alldata = GetAll().ToList();
            var dataFind = new List<Config>();
            try
            {
                dataFind = alldata.FindAll(c => dfrom < DateTime.Parse(c.Timestamp) && DateTime.Parse(c.Timestamp) <= dTo);
            }
            catch(Exception ex)
            {
                _log.Error(sFunc + "Failed to Get Orders | ERROR MSG:" + ex.Message);
            }

            return dataFind;
        }

        public Config Add(Config data)
        {
            string sFunc = "Add - ";

            var allOrders = GetAll();
            bool bAddToDatabase = false;

            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

            try
            {
                var countTest = allOrders.Count();
            }
            catch (Exception ex)
            {
                bAddToDatabase = true;
            }

            if(bAddToDatabase == false)
            {
                var dataFind = allOrders.FirstOrDefault(c => c.Name == data.Name);

                if (dataFind != null)
                {
                    if (dataFind.Value == data.Value)
                        _log.Debug(sFunc + $"Config: {data.Name} already exists in DB (ID:{dataFind.Id})");
                    else
                    {
                        _log.Debug(sFunc + $"Config: {data.Name} exists in DB (ID:{dataFind.Id}) but Value Changed ({dataFind.Value} => {data.Value})");

                        // Transfer all details
                        dataFind.Value = data.Value;
                        dataFind.Timestamp = timestamp;

                        return Update(dataFind);
                    }
                }
                else
                    bAddToDatabase = true;
            }

            if (bAddToDatabase)
            {
                data.Timestamp = timestamp;
                context.Configs.Add(data);

                try
                {
                    context.SaveChanges();
                    _log.Debug(sFunc + $"Config: {data.Name} = {data.Value}");
                }
                catch (Exception ex)
                {
                    _log.Error(sFunc + "Failed Adding to DB | ERROR MSG:" + ex.Message);
                    return null;
                }
            }
            else
                return null;

            return data;
        }

        public bool AddConfigs(JObject configs)
        {
            string sFunc = "AddConfigs - ";

            var allOrders = GetAll();
            bool bResult = true;
            
            IList<string> keys = configs.Properties().Select(p => p.Name).ToList();

            List<Config> dataList = new List<Config>();
            
            foreach (var key in keys)
            {
                var config = new Config()
                {
                    Name = key,
                    Value = configs[key].ToString()
                };

                if (Add(config) == null)
                {
                    bResult = false;
                }
            }

            
            return bResult;
        }

        public Config Update(Config data)
        {
            string sFunc = "Update - ";
            //var x = context.Configs.Attach(data);
            //x.State = EntityState.Modified;
            context.SaveChanges();
            _log.Debug(sFunc + $"Config (Updated):{data.Name} = {data.Value} ");

            return data;
        }
        public Config Delete(int id)
        {
            var x = context.Configs.Find(id);
            if(x != null)
            {
                context.Configs.Remove(x);
                context.SaveChanges();
            }
            return x;
        }
    }
}
