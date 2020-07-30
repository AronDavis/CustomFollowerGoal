using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CustomFollowerGoal.Code;
using CustomFollowerGoal.Code.UserAccessToken;
using CustomFollowerGoal.Models.Subs;
using System.Collections.Generic;

namespace CustomFollowerGoal.Hubs
{
    public class SubsHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly ITwitchApiClient _twitchApiClient;
        private readonly UserAccessTokenStore _userAccessTokenStore;
        public SubsHub(IConfiguration configuration, ITwitchApiClient twitchApiClient, UserAccessTokenStore userAccessTokenStore)
        {
            _configuration = configuration;
            _twitchApiClient = twitchApiClient;
            _userAccessTokenStore = userAccessTokenStore;
        }

        public async Task RequestSubsUpdate() //used to be send message
        {
            int streamId = int.Parse(_configuration.GetSection("AppSettings")["stream-id"]);

            string userAccessToken = _userAccessTokenStore.UserAccessToken;

            if (userAccessToken == null)
                return;

            SubsModel subs = await _twitchApiClient.GetSubs(streamId, userAccessToken);

            List<SubData> subData = new List<SubData>(subs.Data);

            while (subs.Pagination.Cursor != null)
            {
                subs = await _twitchApiClient.GetSubs(streamId, userAccessToken, subs.Pagination.Cursor);
                subData.AddRange(subs.Data);
            }

            await Clients.Caller.SendAsync("UpdateSubs", subData.Count - 1); //used to be receive message
        }
    }
}
