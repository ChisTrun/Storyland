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
        /// <param name="serverIndex" example="0">Index of the server.</param>
        [ProducesResponseType(typeof(Category[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/")]
        public IActionResult GetAllCategories(int serverIndex)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
                if (!isValid) return BadRequest("Invalid server index.");
                return Ok(ScannerController.Instance.sourceScanner.Commands[serverIndex].GetCategories());
            } 
            catch (Exception e) 
            {
                return StatusCode(500, $"Fail to get all categories: {e.Message}.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIndex" example="0">Index of the server.</param>
        /// <param name="categoryId" example="tien-hiep/"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PluginBase.Models.Story[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/{categoryId}/all")]
        public IActionResult GetAllStoriesOfCategory(int serverIndex, string categoryId)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
                if (!isValid) return BadRequest("Invalid server index.");
                return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetStoriesOfCategory(categoryId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories of the category with id {categoryId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get stories of a category with paging.
        /// </summary>
        /// <param name="serverIndex" example="0" >Index of the server.</param>
        /// <param name="categoryId" example="tien-hiep/">Category's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<PluginBase.Models.Story>), 200)]
        [HttpGet]
        [Route("{serverIndex}/{categoryId}")]
        public IActionResult GetAllStoriesOfCategory(int serverIndex, string categoryId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try 
            {
                bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
                if (!isValid) return BadRequest("Invalid server index.");
                return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetStoriesOfCategory(categoryId, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get stories at page {page} of the category with id {categoryId}: {e.Message}.");
            }
        }
    }
}
