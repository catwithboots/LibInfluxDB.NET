using Newtonsoft.Json;

namespace LibInfluxDB.Net.Models
{
    public class Database
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}