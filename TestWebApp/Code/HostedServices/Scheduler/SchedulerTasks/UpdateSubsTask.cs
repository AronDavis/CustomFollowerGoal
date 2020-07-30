using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;
using CustomFollowerGoal.Hubs;
using CustomFollowerGoal.Models.Subs;
using System.Collections.Generic;
using CustomFollowerGoal.Code.UserAccessToken;

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
            string userAccessToken = _userAccessTokenStore.UserAccessToken;

            if (userAccessToken == null)
                return;

            SubsModel subs = await _twitchApiClient.GetSubs(_streamId, userAccessToken);

            List<SubData> subData = new List<SubData>(subs.Data);

            while (subs.Pagination.Cursor != null)
            {
                subs = await _twitchApiClient.GetSubs(_streamId, userAccessToken, subs.Pagination.Cursor);
                subData.AddRange(subs.Data);
            }

            await _hubContext.Clients.All.SendAsync("UpdateSubs", subData.Count); //used to be receive message
        }
    }
}
