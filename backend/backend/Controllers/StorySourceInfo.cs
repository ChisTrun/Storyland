using Microsoft.AspNetCore.Mvc;
using backend.DLLScanner;
using PluginBase.Models;


namespace backend.Controllers
{
    [Route("api/server")]
    public class StorySourceInfo : Controller
    {
        /// <summary>
        /// Get servers.
        /// </summary>
        /// <remarks>        
        /// Get servers information with indexes.
        /// </remarks>
        [ProducesResponseType(typeof(Server[]), 200)]
        [HttpGet]
        public IActionResult GetServers()
        {
            var list = StorySourceScanner.Instance.Commands.Select((com, index) => new Server(index, com.Name));
            return Ok(list);
        }
    }
}
