using Newtonsoft.Json;

namespace CustomFollowerGoal.Models.Subs
{
    public class SubsModel
    {
        [JsonProperty("data")]
        public SubDataModel[] Data { get; set; }

        [JsonProperty("pagination")]
        public PaginationModel Pagination { get; set; }
    }
}
