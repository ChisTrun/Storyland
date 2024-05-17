using Microsoft.AspNetCore.Mvc;
using backend.DLLScanner;
namespace backend.Controllers
{
    [Route("api/server")]
    public class StorySourceInfo : Controller
    {
        [HttpGet]
        public IActionResult GetServers()
        {
            return Ok(StorySourceScanner.Instance.Commands.Select((com, index) => new { name = com.Name, index }));
        }
    }
}
