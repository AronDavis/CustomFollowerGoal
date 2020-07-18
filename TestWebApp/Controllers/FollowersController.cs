using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomFollowerGoal.Models;

namespace CustomFollowerGoal.Controllers
{
    public class FollowersController : Controller
    {
        private readonly ILogger<FollowersController> _logger;

        public FollowersController(ILogger<FollowersController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
