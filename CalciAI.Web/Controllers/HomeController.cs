using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace CalciAI.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ApiControllerBase
    {
        public HomeController(ILogger<HomeController> logger) : base(logger)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult Index()
        {
            return Ok(new
            {
                RemoteIp,
                DeviceType,
                Headers = Request.Headers.Where(x => new[] { "X-Real-IP", "X-Forwarded-For", "X-Forwarded-Proto", "X-Forwarded-Host" }.Contains(x.Key))
            });
        }
    }
}