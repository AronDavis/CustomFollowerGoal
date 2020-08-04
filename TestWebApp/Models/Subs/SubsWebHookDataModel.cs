using Newtonsoft.Json;
using System;

namespace CustomFollowerGoal.Models.Subs
{
    public class SubsWebHookDataModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("event_type")]
        public string EventType { get; set; }

        [JsonProperty("event_timestamp")]
        public DateTime EventTimestamp { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("event_data")]
        public SubDataModel EventData { get; set; }
    }
}
