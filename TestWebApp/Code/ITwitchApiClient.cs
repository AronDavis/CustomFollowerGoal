﻿using System.Net;
using System.Threading.Tasks;
using CustomFollowerGoal.Models;
using CustomFollowerGoal.Models.Follows;
using CustomFollowerGoal.Models.Subs;
using CustomFollowerGoal.Models.WebHooks;

namespace CustomFollowerGoal.Code
{
    public interface ITwitchApiClient
    {
        Task<HttpStatusCode> SetWebHook(WebHooksModel model, string oauthOverride = null);
        Task<FollowsModel> GetFollows(int toId, string oauthToken);
        Task<SubsModel> GetSubs(int broadcasterId, string userAccessToken, string after = null);
        Task<RefreshableUserAccessTokenModel> RefreshUserAccessToken(string refreshToken);
    }
}
