using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomFollowerGoal.Code.HostedServices.Scheduler
{
    public class SchedulerService : HostedService
    {
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;
        private readonly List<SchedulerTask> _tasks = new List<SchedulerTask>();

        public SchedulerService(IEnumerable<IScheduledTask> tasks)
        {
            var utcNow = DateTime.UtcNow;

            foreach (var scheduledTask in tasks)
            {
                _tasks.Add(new SchedulerTask
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTimeUtc = utcNow
                });
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _tasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await taskThatShouldRun.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }
    }
}
