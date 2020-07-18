using Newtonsoft.Json;

namespace CustomFollowerGoal.Models.Follows
{
    public class FollowsModel
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("data")]
        public FollowData[] Data { get; set; }
    }
}
