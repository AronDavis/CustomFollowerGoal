using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace CustomFollowerGoal.Code.HostedServices
{
    public abstract class HostedService : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _cancellationTokenSource;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //create a linked token so we can trigger cancellation outside of this token's cancellation
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            //store the task we're executing
            _executingTask = ExecuteAsync(_cancellationTokenSource.Token);

            //if the task is completed then return it, otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //stop called without start
            if (_executingTask == null)
            {
                return;
            }

            //signal cancellation to the executing method
            _cancellationTokenSource.Cancel();

            //wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            //throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }

        //derived classes should override this and execute a long running method until cancellation is requested
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
