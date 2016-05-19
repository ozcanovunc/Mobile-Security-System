using Newtonsoft.Json;

namespace Mobile_Security_System.Models
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
