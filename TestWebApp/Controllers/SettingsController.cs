using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CustomFollowerGoal.Hubs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomFollowerGoal.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IHubContext<FollowersHub> _hubContext;
        public SettingsController(IHubContext<FollowersHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // POST api/<SettingsController>/FollowerGoal
        [HttpPost]
        public async Task<IActionResult> FollowerGoal([FromBody] int followerGoal)
        {
            if (followerGoal <= 0)
                return BadRequest("Follower goal must be greater than 0.");

            await _hubContext.Clients.All.SendAsync("UpdateFollowerGoal", followerGoal);

            return Ok();
        }
    }
}
