using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS.Service.Models
{
    public class MailModel
    {
        [JsonProperty("Image")]
        public string Image { get; set; }

        [JsonProperty("Subject")]
        public string Subject { get; set; }

        [JsonProperty("Body")]
        public string Body { get; set; }

        [JsonProperty("To")]
        public string To { get; set; }
    }
}