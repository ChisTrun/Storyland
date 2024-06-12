using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;
using System.Diagnostics;


namespace backend.Controllers
{
    [Route("api/search")]
    public class Searching : Controller
    {
        /// <summary>
        /// Get all stories of an Author.
        /// </summary>
        /// <param name="serverID" example="0">ID of the server.</param>
        /// <param name="authorId" example="vainy">Author's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("{serverID}/tacgia/{authorId}/all")]
        public IActionResult SearchByAuthor(string serverID, string authorId)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerID(serverID);
                if (!isValid) return BadRequest("Invalid server index.");
                return Ok(StorySourceScanner.Instance.Commands[serverID].Item1.GetStoriesOfAuthor(authorId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories of the author with id {authorId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get stories of an Author with paging.
        /// </summary>
        /// <param name="serverID" example="0">ID of the server.</param>
        /// <param name="authorId" example="vainy">Author's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<PluginBase.Models.Story>), 200)]
        [HttpGet]
        [Route("{serverID}/tacgia/{authorId}")]
        public IActionResult SearchByAuthor(String serverID, string authorId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerID(serverID);
                if (!isValid) return BadRequest("Invalid server index.");
                return Ok(StorySourceScanner.Instance.Commands[serverID].Item1.GetStoriesOfAuthor(authorId, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories at page {page} of the author with id  {authorId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get all stories by searching.
        /// </summary>
        /// <param name="serverID" example="0">ID of the server.</param>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("{serverID}/truyen/{storyName}/all")]
        public IActionResult SearchByStoryName(string serverID, string storyName)
        {
            try 
            {
                bool isValid = Handler.ServerHandler.CheckServerID(serverID);
                if (!isValid) return BadRequest("Invalid server index.");
                return Ok(StorySourceScanner.Instance.Commands[serverID].Item1.GetStoriesBySearchName(storyName));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories by searching with keyword {storyName}: {e.Message}.");
            }
            
        }

        /// <summary>
        /// Get stories by searching with paging.
        /// </summary>
        /// <param name="serverID" example="0">ID of the server.</param>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<PluginBase.Models.Story>), 200)]
        [HttpGet]
        [Route("{serverID}/truyen/{storyName}")]
        public IActionResult SearchByStoryName(string serverID, string storyName, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerID(serverID);
                if (!isValid) return BadRequest("Invalid server index.");
                return Ok(StorySourceScanner.Instance.Commands[serverID].Item1.GetStoriesBySearchName(storyName, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories at page {page} by searching with keyword {storyName}: {e.Message}.");
            }
        }
    }
}
