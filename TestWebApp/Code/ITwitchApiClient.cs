using System.Net;
using System.Threading.Tasks;
using CustomFollowerGoal.Models.Follows;
using CustomFollowerGoal.Models.Subs;
using CustomFollowerGoal.Models.WebHooks;

namespace CustomFollowerGoal.Code
{
    public interface ITwitchApiClient
    {
        Task<HttpStatusCode> SetWebHook(WebHooksModel model);

        Task<FollowsModel> GetFollows(int toId);
        Task<SubsModel> GetSubs(int broadcasterId, string userAccessToken, string after = null);
    }
}
