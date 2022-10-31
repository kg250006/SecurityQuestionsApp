using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBService.DB.Models
{
    public class Config
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }
        public string Name { set; get; }
        public string Value { set; get; }
        public string Timestamp { set; get; }

        public Config(string name,JObject config)
        {
            Name = name;
            Value = config.ToString();

            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public Config()
        {

        }

    }
}
