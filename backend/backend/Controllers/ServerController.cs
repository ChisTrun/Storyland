using backend.Application.Services.Abstract;
using backend.Domain.Mics;
using Microsoft.AspNetCore.Mvc;


namespace backend.Controllers
{
    [Route("api/server")]
    public class ServerController : Controller
    {
        private readonly ICrawlingService _crawlingService;

        public ServerController(ICrawlingService crawlingService)
        {
            _crawlingService = crawlingService;
        }

        /// <summary>
        /// Get servers.
        /// </summary>
        [ProducesResponseType(typeof(PluginInfo[]), 200)]
        [HttpGet]
        public IActionResult GetServers()
        {
            try
            {
                return Ok(_crawlingService.GetServers());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get servers: {e.Message}.");
            }
        }
    }
}
