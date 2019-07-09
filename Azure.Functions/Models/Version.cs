using Newtonsoft.Json;
using System;

namespace SetVersion.Models
{
    public class Version
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
