using System.Threading;
using System.Threading.Tasks;
using CustomFollowerGoal.Code.UserAccessToken;
using CustomFollowerGoal.Models;

namespace CustomFollowerGoal.Code.HostedServices.Scheduler.SchedulerTasks
{
    public class RefreshUserAccessTokenTask : IScheduledTask
    {
        public string Schedule => "*/30 * * * *"; //every 30 minutes

        private readonly ITwitchApiClient _twitchApiClient;
        private readonly UserAccessTokenStore _userAccessTokenStore;

        public RefreshUserAccessTokenTask(ITwitchApiClient twitchApiClient, UserAccessTokenStore userAccessTokenStore)
        {
            _twitchApiClient = twitchApiClient;
            _userAccessTokenStore = userAccessTokenStore;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_userAccessTokenStore.UserAccessToken == null)
                return;

            RefreshableUserAccessTokenModel model = await _twitchApiClient.RefreshUserAccessToken(_userAccessTokenStore.UserAccessToken.RefreshToken);

            RefreshableUserAccessToken refreshableUserAccessToken = new RefreshableUserAccessToken()
            {
                AccessToken = model.AccessToken,
                RefreshToken = model.RefreshToken
            };

            Interlocked.Exchange(ref _userAccessTokenStore.UserAccessToken, refreshableUserAccessToken);
        }
    }
}
