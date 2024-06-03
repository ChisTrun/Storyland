﻿using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;


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
            try
            {
                var list = StorySourceScanner.Instance.Commands.Select((com, index) => new Server(index, com.Name));
                return Ok(list);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get servers: {e.Message}.");
            }
            
        }
    }
}
