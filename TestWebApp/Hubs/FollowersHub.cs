using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CustomFollowerGoal.Code;
using CustomFollowerGoal.Code.UserAccessToken;

namespace CustomFollowerGoal.Hubs
{
    public class FollowersHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly ITwitchApiClient _twitchApiClient;
        private readonly UserAccessTokenStore _userAccessTokenStore;

        public FollowersHub(IConfiguration configuration, ITwitchApiClient twitchApiClient, UserAccessTokenStore userAccessTokenStore)
        {
            _configuration = configuration;
            _twitchApiClient = twitchApiClient;
            _userAccessTokenStore = userAccessTokenStore;
        }

        public async Task RequestFollowersUpdate() //used to be send message
        {
            var streamId = _configuration.GetSection("AppSettings")["stream-id"];
            var follows = await _twitchApiClient.GetFollows(int.Parse(streamId), _userAccessTokenStore?.UserAccessToken?.AccessToken);
            await Clients.Caller.SendAsync("UpdateFollowers", follows.Total); //used to be receive message
        }
    }
}
