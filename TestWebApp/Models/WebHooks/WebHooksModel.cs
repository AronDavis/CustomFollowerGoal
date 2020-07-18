using Newtonsoft.Json;

namespace CustomFollowerGoal.Models.WebHooks
{
    public class WebHooksModel
    {
        [JsonProperty("hub.callback")]
        public string Callback { get; set; }

        [JsonProperty("hub.mode")]
        public string Mode { get; set; }

        [JsonProperty("hub.topic")]
        public string Topic { get; set; }

        [JsonProperty("hub.lease_seconds")]
        public int LeaseSeconds { get; set; }
    }
}
