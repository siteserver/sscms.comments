using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Comments.Controllers
{
    [Route("api/comments/ping")]
    public class PingController : ControllerBase
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public string Get()
        {
            return "pong";
        }
    }
}