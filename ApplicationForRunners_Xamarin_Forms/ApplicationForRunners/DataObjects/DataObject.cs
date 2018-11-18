using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace ApplicationForRunners.DataObjects
{
    public class DataObject
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Version]
        public string Version { get; set; }

        public string UserId { get; set; }
    }
}
