using System;
using Newtonsoft.Json;

namespace exercise_one.Models
{
    public class Item
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Text { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}