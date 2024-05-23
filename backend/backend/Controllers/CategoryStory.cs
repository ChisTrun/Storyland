using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.Handler;

namespace backend.Controllers
{
    [Route("api/category")]
    public class CategoryStory : Controller
    {
        /// <summary>
        /// Get all categories.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        [ProducesResponseType(typeof(Category[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/")] 
        public IActionResult GetAllCategories(int serverIndex)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetCategories());
        }

        /// <summary>
        /// Get all stories of a category.
        /// </summary>
        /// <param name="categoryId" example="truyen-tien-hiep/">Category's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/{categoryId}/all")]
        public IActionResult GetAllStoriesOfCategory(int serverIndex,string categoryId)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetStoriesOfCategory(categoryId));
        }

        /// <summary>
        /// Get stories of a category with paging.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="categoryId" example="truyen-tien-hiep/">Category's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<PluginBase.Models.Story>), 200)]
        [HttpGet]
        [Route("{serverIndex}/{categoryId}")]
        public IActionResult GetAllStoriesOfCategory(int serverIndex, string categoryId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetStoriesOfCategory(categoryId, page, limit));
        }
    }
}
