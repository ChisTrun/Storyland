using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;

namespace backend.Controllers
{
    [Route("api/category")]
    public class CategoryStory : Controller
    {
        /// <summary>
        /// Get all categories.
        /// </summary>
        [ProducesResponseType(typeof(Category[]), 200)]
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetCategories());
        }

        /// <summary>
        /// Get all stories of a category.
        /// </summary>
        /// <param name="categoryId" example="?ctg=1">Category's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("{categoryId}/all")]
        public IActionResult GetAllStoriesOfCategory(string categoryId)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesOfCategory(categoryId));
        }

        /// <summary>
        /// Get stories of a category with paging.
        /// </summary>
        /// <param name="categoryId" example="?ctg=1">Category's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<PluginBase.Models.Story>), 200)]
        [HttpGet]
        [Route("{categoryId}")]
        public IActionResult GetAllStoriesOfCategory(string categoryId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesOfCategory(categoryId, page, limit));
        }
    }
}
