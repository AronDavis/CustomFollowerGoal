using System.Threading;
using System.Threading.Tasks;

namespace CustomFollowerGoal.Code.HostedServices.Scheduler
{
    public interface IScheduledTask
    {
        string Schedule { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
