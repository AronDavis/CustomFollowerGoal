using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CustomFollowerGoal.Code;
using CustomFollowerGoal.Code.UserAccessToken;
using CustomFollowerGoal.Code.Services;

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
            SubsService subsService = new SubsService(_twitchApiClient, _userAccessTokenStore);
            int streamId = int.Parse(_configuration.GetSection("AppSettings")["stream-id"]);
            int subCount = await subsService.GetSubsCountAsync(streamId);

            await Clients.Caller.SendAsync("UpdateSubs", subCount); //used to be receive message
        }
    }
}
