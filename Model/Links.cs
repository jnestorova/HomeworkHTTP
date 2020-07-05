using Newtonsoft.Json;
using System;

namespace HomeworkHTTP.Model
{
    public partial class Link
    {
        [JsonProperty("rel", NullValueHandling = NullValueHandling.Ignore)]
        public string Rel { get; set; }

        [JsonProperty("href", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Href { get; set; }
    }
}
