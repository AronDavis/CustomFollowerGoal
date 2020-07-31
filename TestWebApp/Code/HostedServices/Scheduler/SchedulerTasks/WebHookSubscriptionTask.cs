using System.Threading;
using System.Threading.Tasks;
using CustomFollowerGoal.Models.WebHooks;

namespace CustomFollowerGoal.Code.HostedServices.Scheduler.SchedulerTasks
{
    public class WebHookSubscriptionTask : IScheduledTask
    {
        public string Schedule => "0 0 * * *"; //every day at midnight...should probably make this configurable via the constructor

        private readonly ITwitchApiClient _twitchApiClient;
        private readonly WebHooksModel _model;
        private readonly string _oauthOverride;

        public WebHookSubscriptionTask(ITwitchApiClient twitchApiClient, WebHooksModel model, string oauthOverride = null)
        {
            _twitchApiClient = twitchApiClient;
            _model = model;
            _oauthOverride = oauthOverride;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _twitchApiClient.SetWebHook(_model, _oauthOverride);
        }
    }
}
