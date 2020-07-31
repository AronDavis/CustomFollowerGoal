using Newtonsoft.Json;

namespace CustomFollowerGoal.Models.Subs
{
    public class SubsWebHookModel
    {
        [JsonProperty("data")]
        public SubsWebHookDataModel[] Data { get; set; }
    }
}
