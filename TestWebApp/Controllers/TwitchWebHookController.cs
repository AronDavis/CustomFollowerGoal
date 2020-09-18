using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using CustomFollowerGoal.Code;
using CustomFollowerGoal.Hubs;
using CustomFollowerGoal.Models.Follows;
using CustomFollowerGoal.Models.Subs;
using CustomFollowerGoal.Code.Services;
using CustomFollowerGoal.Code.UserAccessToken;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomFollowerGoal.Controllers
{
    /// <summary>
    /// This is really just a webhook for the follows stuff.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TwitchWebHookController : ControllerBase
    {
        private readonly IHubContext<FollowersHub> _followersHubContext;
        private readonly IHubContext<SubsHub> _subsHubContext;
        private readonly ITwitchApiClient _twitchApiClient;
        private readonly UserAccessTokenStore _userAccessTokenStore;

        public TwitchWebHookController(
            IHubContext<FollowersHub> followersHubContext, 
            IHubContext<SubsHub> subsHubContext, 
            ITwitchApiClient twitchApiClient,
            UserAccessTokenStore userAccessTokenStore)
        {
            _followersHubContext = followersHubContext;
            _subsHubContext = subsHubContext;
            _twitchApiClient = twitchApiClient;
            _userAccessTokenStore = userAccessTokenStore;
        }


        // GET: api/<TwitchWebHookController>
        /// <summary>
        /// Handles validating the webhook with Twitch.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="topic"></param>
        /// <param name="challenge"></param>
        /// <param name="leaseSeconds"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("follows")]
        [Route("subs")]
        public IActionResult Get(
            [FromQuery(Name = "hub.mode")]string mode,
            [FromQuery(Name = "hub.topic")] string topic,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.lease_seconds")] int leaseSeconds
            )
        {
            return Ok(challenge);
        }

        [HttpPost]
        [Route("follows")]
        public async Task<IActionResult> Follows(FollowsWebHookModel newFollows)
        {
            var data = newFollows.Data[0];
            var follows = await _twitchApiClient.GetFollows(data.ToId, _userAccessTokenStore?.UserAccessToken?.AccessToken);
            await _followersHubContext.Clients.All.SendAsync("UpdateFollowers", follows.Total);

            return Ok();
        }

        [HttpPost]
        [Route("subs")]
        public async Task<IActionResult> Subs(SubsWebHookModel newSubs)
        {
            var data = newSubs.Data[0];

            SubsService subsService = new SubsService(_twitchApiClient, _userAccessTokenStore);
            int subCount = await subsService.GetSubsCountAsync(data.EventData.BroadcasterId);

            await _subsHubContext.Clients.All.SendAsync("UpdateSubs", subCount); //used to be receive message

            return Ok();
        }
    }
}
