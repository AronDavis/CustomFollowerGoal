using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;
using CustomFollowerGoal.Hubs;
using CustomFollowerGoal.Code.UserAccessToken;

namespace CustomFollowerGoal.Code.HostedServices.Scheduler.SchedulerTasks
{
    public class UpdateFollowersTask : IScheduledTask
    {
        public string Schedule => "0 * * * *"; //every hour

        private readonly IHubContext<FollowersHub> _hubContext;
        private readonly ITwitchApiClient _twitchApiClient;
        private readonly UserAccessTokenStore _userAccessTokenStore;
        private readonly int _toId;

        public UpdateFollowersTask(IHubContext<FollowersHub> hubContext, ITwitchApiClient twitchApiClient, UserAccessTokenStore userAccessTokenStore, int toId)
        {
            _hubContext = hubContext;
            _twitchApiClient = twitchApiClient;
            _userAccessTokenStore = userAccessTokenStore;
            _toId = toId;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var follows = await _twitchApiClient.GetFollows(_toId, _userAccessTokenStore?.UserAccessToken?.AccessToken);
            await _hubContext.Clients.All.SendAsync("UpdateFollowers", follows.Total);
        }
    }
}
