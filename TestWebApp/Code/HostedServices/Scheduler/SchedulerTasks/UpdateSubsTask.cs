using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;
using CustomFollowerGoal.Hubs;
using CustomFollowerGoal.Code.UserAccessToken;
using CustomFollowerGoal.Code.Services;

namespace CustomFollowerGoal.Code.HostedServices.Scheduler.SchedulerTasks
{
    public class UpdateSubsTask : IScheduledTask
    {
        public string Schedule => "0 * * * *"; //every hour

        private readonly IHubContext<SubsHub> _hubContext;
        private readonly ITwitchApiClient _twitchApiClient;
        private readonly UserAccessTokenStore _userAccessTokenStore;
        private readonly int _streamId;

        public UpdateSubsTask(IHubContext<SubsHub> hubContext, ITwitchApiClient twitchApiClient, UserAccessTokenStore userAccessTokenStore, int streamId)
        {
            _hubContext = hubContext;
            _twitchApiClient = twitchApiClient;
            _userAccessTokenStore = userAccessTokenStore;
            _streamId = streamId;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            SubsService subsService = new SubsService(_twitchApiClient, _userAccessTokenStore);
            int subCount = await subsService.GetSubsCountAsync(_streamId);

            await _hubContext.Clients.All.SendAsync("UpdateSubs", subCount); //used to be receive message
        }
    }
}
