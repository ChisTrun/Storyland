using Microsoft.AspNetCore.Mvc;
using backend.DLLScanner;
using PluginBase.Contract;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Controllers
{
    class ServerInfo(int index, string name)
    {
        public int Index { get; set; } = index;
        public string Name { get; set; } = name;
    }

    [Route("api/server")]
    public class StorySourceInfo : Controller
    {
        /// <summary>
        /// Get servers.
        /// </summary>
        /// <remarks>        
        /// Get servers information with indexes.
        /// </remarks>
        [ProducesResponseType(typeof(ServerInfo[]), 200)]
        [HttpGet]
        public IActionResult GetServers()
        {
            var list = StorySourceScanner.Instance.Commands.Select((com, index) => new ServerInfo(index, com.Name));
            return Ok(list);
        }
    }
}
