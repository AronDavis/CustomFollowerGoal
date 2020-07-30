using Newtonsoft.Json;
using System;

namespace CustomFollowerGoal.Models.Subs
{
    public class SubData
    {
        [JsonProperty("broadcaster_id")]
        public int BroadcasterId { get; set; }

        [JsonProperty("broadcaster_name")]
        public string BroadcasterName { get; set; }

        [JsonProperty("is_gift")]
        public bool IsGift { get; set; }

        [JsonProperty("tier")]
        public int Tier { get; set; }

        [JsonProperty("plan_name")]
        public string PlanName { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }
}
