using NCrontab;
using System;

namespace CustomFollowerGoal.Code.HostedServices.Scheduler
{
    public class SchedulerTask
    {
        public CrontabSchedule Schedule;
        public IScheduledTask Task;
        public DateTime LastRunTimeUtc;
        public DateTime NextRunTimeUtc;
        
        public void Increment()
        {
            LastRunTimeUtc = NextRunTimeUtc;
            NextRunTimeUtc = Schedule.GetNextOccurrence(NextRunTimeUtc);
        }

        public bool ShouldRun(DateTime currentTimeUtc)
        {
            return NextRunTimeUtc < currentTimeUtc && LastRunTimeUtc != NextRunTimeUtc;
        }
    }
}
