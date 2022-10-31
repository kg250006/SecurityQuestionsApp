using System;
using System.Collections.Generic;
using System.Text;

namespace DBService.DB.Models
{
    public interface IConfigRepository
    {
        IEnumerable<Config> GetAll();
        List<Config> GetByTimestamp(DateTime dfrom, DateTime dTo);
        Config GetById(int Id);
        public string GetValue(string name);
        Config Add(Config data);
        Config Update(Config data);
        Config Delete(int Id);

    }
}
