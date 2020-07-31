using Newtonsoft.Json;

namespace CustomFollowerGoal.Models.Follows
{

    public class FollowsWebHookModel
    {
        [JsonProperty("data")]
        public FollowDataModel[] Data { get; set; }
    }
}
