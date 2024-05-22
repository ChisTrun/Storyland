using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.Handler;


namespace backend.Controllers
{
    [Route("api/search")]
    public class Searching : Controller
    {
        /// <summary>
        /// Get all stories of an Author.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="authorId" example="?author=27">Author's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/tacgia/{authorId}/all")]
        public IActionResult SearchByAuthor(int serverIndex,string authorId)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetStoriesOfAuthor(authorId));
        }

        /// <summary>
        /// Get stories of an Author with paging.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="authorId" example="?author=27">Author's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<PluginBase.Models.Story>), 200)]
        [HttpGet]
        [Route("{serverIndex}/tacgia/{authorId}")]
        public IActionResult SearchByAuthor(int serverIndex, string authorId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetStoriesOfAuthor(authorId, page, limit));
        }

        /// <summary>
        /// Get all stories by searching.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/truyen/{storyName}/all")]
        public IActionResult SearchByStoryName(int serverIndex, string storyName)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetStoriesBySearchName(storyName));
        }

        /// <summary>
        /// Get stories by searching with paging.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<PluginBase.Models.Story>), 200)]
        [HttpGet]
        [Route("{serverIndex}/truyen/{storyName}")]
        public IActionResult SearchByStoryName(int serverIndex, string storyName, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetStoriesBySearchName(storyName, page, limit));
        }
    }
}
