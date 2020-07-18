using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;
using CustomFollowerGoal.Hubs;

namespace CustomFollowerGoal.Code.HostedServices.Scheduler.SchedulerTasks
{
    public class UpdateFollowersTask : IScheduledTask
    {
        public string Schedule => "0 * * * *"; //every hour

        private readonly IHubContext<FollowersHub> _hubContext;
        private readonly ITwitchApiClient _twitchApiClient;
        private readonly int _toId;

        public UpdateFollowersTask(IHubContext<FollowersHub> hubContext, ITwitchApiClient twitchApiClient, int toId)
        {
            _twitchApiClient = twitchApiClient;
            _hubContext = hubContext;
            _toId = toId;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var follows = await _twitchApiClient.GetFollows(_toId);
            await _hubContext.Clients.All.SendAsync("UpdateFollowers", follows.Total);
        }
    }
}
