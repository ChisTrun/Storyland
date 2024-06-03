using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;

namespace backend.Controllers
{
    [Route("api/author/search")]
    public class Author : Controller
    {/// <summary>
     /// Get all Authors by search key.
     /// </summary>
     /// <param name="serverIndex" example="0">Index of the server.</param>
     /// <param name="authorName" example="Đỉnh">Story name's searching keyword.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Author[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/author/{authorName}/all")]
        public IActionResult SearchAuthorsByName(int serverIndex, string authorName)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid)
                return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetAuthorsBySearchName(authorName));
        }

        /// <summary>
        /// Get stories by searching with paging.
        /// </summary>
        /// <param name="serverIndex" example="0">Index of the server.</param>
        /// <param name="authorName" example="Đỉnh">Story name's searching keyword.</param>
        /// <param name="page" example="1">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<PluginBase.Models.Author>), 200)]
        [HttpGet]
        [Route("{serverIndex}/truyen/{storyName}")]
        public IActionResult SearchAuthorsByName(int serverIndex, string authorName, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid)
                return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetAuthorsBySearchName(authorName, page, limit));
        }
    }
}
