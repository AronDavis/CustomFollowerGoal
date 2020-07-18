using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using CustomFollowerGoal.Code;
using CustomFollowerGoal.Hubs;
using CustomFollowerGoal.Models.Follows;

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
        private readonly IHubContext<FollowersHub> _hubContext;
        private readonly ITwitchApiClient _twitchApiClient;

        public TwitchWebHookController(IHubContext<FollowersHub> hubContext, ITwitchApiClient twitchApiClient)
        {
            _hubContext = hubContext;
            _twitchApiClient = twitchApiClient;
        }


        // GET: api/<TwitchWebHookController>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery(Name = "hub.mode")]string mode,
            [FromQuery(Name = "hub.topic")] string topic,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.lease_seconds")] int leaseSeconds
            )
        {
            return Ok(challenge);
        }

        [HttpPost]
        public async Task<IActionResult> Post(FollowsWebHookModel newFollows)
        {
            //...can probably just take the first one
            foreach (var data in newFollows.Data)
            {
                var follows = await _twitchApiClient.GetFollows(data.ToId);
                await _hubContext.Clients.All.SendAsync("UpdateFollowers", follows.Total);
            }

            return Ok();
        }
    }
}
