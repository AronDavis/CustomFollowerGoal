using System.Collections.Concurrent;

namespace CustomFollowerGoal.Code.UserAccessToken
{
    public class UserAccessStateStore
    {
        public readonly ConcurrentDictionary<string, string> CurrentStates = new ConcurrentDictionary<string, string>();
    }
}
