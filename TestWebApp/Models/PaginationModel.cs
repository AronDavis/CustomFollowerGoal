using Newtonsoft.Json;

namespace CustomFollowerGoal.Models
{
    public class PaginationModel
    {
        [JsonProperty("cursor")]
        public string Cursor { get; set; }
    }
}
