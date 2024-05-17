using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/search")]
    public class Searching : Controller
    {
        /// <summary>
        /// Get all stories of an Author.
        /// </summary>
        /// <param name="authorId" example="?author=27">Author's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("tacgia/{authorId}/all")]
        public IActionResult SearchByAuthor(string authorId)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesOfAuthor(authorId));
        }

        /// <summary>
        /// Get stories of an Author with paging.
        /// </summary>
        /// <param name="authorId" example="?author=27">Author's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("tacgia/{authorId}")]
        public IActionResult SearchByAuthor(string authorId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesOfAuthor(authorId, page, limit));
        }

        /// <summary>
        /// Get all stories by searching.
        /// </summary>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("truyen/{storyName}/all")]
        public IActionResult SearchByStoryName(string storyName)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesBySearchName(storyName));
        }

        /// <summary>
        /// Get stories by searching with paging.
        /// </summary>
        /// <param name="storyName" example="Đỉnh">Story name's searching keyword.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("truyen/{storyName}")]
        public IActionResult SearchByStoryName(string storyName, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesBySearchName(storyName, page, limit));
        }
    }
}
