using Newtonsoft.Json;

namespace CustomFollowerGoal.Models.Subs
{
    public class SubsModel
    {
        [JsonProperty("data")]
        public SubData[] Data { get; set; }

        [JsonProperty("pagination")]
        public PaginationModel Pagination { get; set; }
    }
}
