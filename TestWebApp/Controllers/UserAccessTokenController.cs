using CustomFollowerGoal.Code.UserAccessToken;
using CustomFollowerGoal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CustomFollowerGoal.Controllers
{
    public class UserAccessTokenController : Controller
    {
        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly UserAccessStateStore _userAccessStateStore;
        private readonly UserAccessTokenStore _userAccessTokenStore;

        public UserAccessTokenController(
            IConfiguration configuration,
            UserAccessStateStore userAccessStateStore,
            UserAccessTokenStore userAccessTokenStore
            )
        {
            IConfigurationSection appSettings = configuration.GetSection("AppSettings");

            _clientId = appSettings["api-client-id"];
            _clientSecret = appSettings["api-client-secret"];

            _userAccessStateStore = userAccessStateStore;
            _userAccessTokenStore = userAccessTokenStore;
        }
        
        public IActionResult Auth()
        {
            var url = "https://id.twitch.tv/oauth2/authorize";

            url = QueryHelpers.AddQueryString(url, "client_id", _clientId);
            url = QueryHelpers.AddQueryString(url, "redirect_uri", $"http://{HttpContext.Request.Host.Host.ToLower()}/useraccesstoken");
            url = QueryHelpers.AddQueryString(url, "response_type", "code");
            url = QueryHelpers.AddQueryString(url, "scope", "channel:read:subscriptions");

            var state = Guid.NewGuid().ToString().ToLower();
            _userAccessStateStore.CurrentStates.AddOrUpdate(state, state, (key, oldValue) => state);
            url = QueryHelpers.AddQueryString(url, "state", state);

            return Redirect(url);
        }

        public async Task<IActionResult> Index(string code, string state)
        {
            if (state == null || !_userAccessStateStore.CurrentStates.ContainsKey(state))
                return BadRequest("You're a naughty poo poo boy aren't ya, you little fuck?");

            _userAccessStateStore.CurrentStates.TryRemove(state, out string removedState);

            HttpClient client = new HttpClient();

            string url = "https://id.twitch.tv/oauth2/token";

            url = QueryHelpers.AddQueryString(url, "client_id", _clientId);
            url = QueryHelpers.AddQueryString(url, "client_secret", _clientSecret);
            url = QueryHelpers.AddQueryString(url, "code", code);
            url = QueryHelpers.AddQueryString(url, "grant_type", "authorization_code");
            url = QueryHelpers.AddQueryString(url, "redirect_uri", $"http://{HttpContext.Request.Host.Host.ToLower()}/useraccesstoken");

            HttpResponseMessage response = await client.PostAsync(url, null);
            string jsonString = await response.Content.ReadAsStringAsync();
            UserAccessTokenModel data = JsonConvert.DeserializeObject<UserAccessTokenModel>(jsonString);

            if (data?.AccessToken == null)
                return BadRequest("Failed to authorize.");

            var refreshableToken = new RefreshableUserAccessToken()
            {
                AccessToken = data.AccessToken,
                RefreshToken = data.RefreshToken
            };

            Interlocked.Exchange(ref _userAccessTokenStore.UserAccessToken, refreshableToken);

            return Ok("Auth Successful!");
        }
    }
}
