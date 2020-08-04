using CustomFollowerGoal.Code.UserAccessToken;
using CustomFollowerGoal.Models.Subs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomFollowerGoal.Code.Services
{
    public class SubsService
    {
        private readonly ITwitchApiClient _twitchApiClient;
        private readonly UserAccessTokenStore _userAccessTokenStore;
        public SubsService(ITwitchApiClient twitchApiClient, UserAccessTokenStore userAccessTokenStore)
        {
            _twitchApiClient = twitchApiClient;
            _userAccessTokenStore = userAccessTokenStore;
        }

        public async Task<int> GetSubsCountAsync(int streamId)
        {
            string userAccessToken = _userAccessTokenStore?.UserAccessToken?.AccessToken;

            if (userAccessToken == null)
                return -1;

            SubsModel subs = await _twitchApiClient.GetSubs(streamId, userAccessToken);

            List<SubDataModel> subData = new List<SubDataModel>(subs.Data);

            while (subs.Pagination.Cursor != null)
            {
                subs = await _twitchApiClient.GetSubs(streamId, userAccessToken, subs.Pagination.Cursor);
                subData.AddRange(subs.Data);
            }

            return subData.Count - 1;
        }
    }
}
