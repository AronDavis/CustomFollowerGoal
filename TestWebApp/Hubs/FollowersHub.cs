using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CustomFollowerGoal.Code;

namespace CustomFollowerGoal.Hubs
{
    public class FollowersHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly ITwitchApiClient _twitchApiClient;
        public FollowersHub(IConfiguration configuration, ITwitchApiClient twitchApiClient)
        {
            _configuration = configuration;
            _twitchApiClient = twitchApiClient;
        }

        public async Task RequestFollowersUpdate() //used to be send message
        {
            var streamId = _configuration.GetSection("AppSettings")["stream-id"];
            var follows = await _twitchApiClient.GetFollows(int.Parse(streamId));
            await Clients.Caller.SendAsync("UpdateFollowers", follows.Total); //used to be receive message
        }
    }
}
