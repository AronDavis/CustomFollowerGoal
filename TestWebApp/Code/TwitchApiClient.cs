using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CustomFollowerGoal.Models.Follows;
using CustomFollowerGoal.Models.WebHooks;
using CustomFollowerGoal.Models.Subs;
using System;
using CustomFollowerGoal.Models;

namespace CustomFollowerGoal.Code
{
    public class TwitchApiClient : ITwitchApiClient
    {
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public TwitchApiClient(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("client-id", _clientId);
        }

        public async Task<HttpStatusCode> SetWebHook(WebHooksModel model, string oauthToken)
        {
            var url = "https://api.twitch.tv/helix/webhooks/hub";

            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.SendAsync(new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Headers = {
                    { "Authorization", $"Bearer {oauthToken}" }
                },
                Content = data
            });

            string shit = await response.Content.ReadAsStringAsync();

            return response.StatusCode;
        }

        public async Task<FollowsModel> GetFollows(int toId, string oauthToken)
        {
            var url = "https://api.twitch.tv/helix/users/follows";

            url = QueryHelpers.AddQueryString(url, "first", 1.ToString()); //returns only 1 follow in the data object (we don't need more than that for now)

            url = QueryHelpers.AddQueryString(url, "to_id", toId.ToString());

            HttpResponseMessage response = await _client.SendAsync(new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
                Headers = {
                    { "Authorization", $"Bearer {oauthToken}" }
                }
            });

            string jsonString = await response.Content.ReadAsStringAsync();
            FollowsModel data = JsonConvert.DeserializeObject<FollowsModel>(jsonString);

            return data;
        }

        public async Task<SubsModel> GetSubs(int broadcasterId, string userAccessToken, string cursor = null)
        {
            var url = "https://api.twitch.tv/helix/subscriptions";

            url = QueryHelpers.AddQueryString(url, "broadcaster_id", broadcasterId.ToString());

            if(cursor != null)
                url = QueryHelpers.AddQueryString(url, "after", cursor);

            HttpResponseMessage response = await _client.SendAsync(new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
                Headers = {
                    { "Authorization", $"Bearer {userAccessToken}" }
                }
            });

            string jsonString = await response.Content.ReadAsStringAsync();
            SubsModel data = JsonConvert.DeserializeObject<SubsModel>(jsonString);

            return data;
        }

        public async Task<RefreshableUserAccessTokenModel> RefreshUserAccessToken(string refreshToken)
        {
            HttpClient client = new HttpClient();

            string url = "https://id.twitch.tv/oauth2/token";

            url = QueryHelpers.AddQueryString(url, "client_id", _clientId);
            url = QueryHelpers.AddQueryString(url, "client_secret", _clientSecret);
            url = QueryHelpers.AddQueryString(url, "grant_type", "refresh_token");
            url = QueryHelpers.AddQueryString(url, "refresh_token", refreshToken);

            HttpResponseMessage response = await client.PostAsync(url, null);
            string jsonString = await response.Content.ReadAsStringAsync();
            RefreshableUserAccessTokenModel data = JsonConvert.DeserializeObject<RefreshableUserAccessTokenModel>(jsonString);

            return data;
        }
    }
}
