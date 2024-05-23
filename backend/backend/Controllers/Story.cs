using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.Handler;



namespace backend.Controllers
{
    [Route("api/story")]
    public class Story : Controller
    {
        /// <summary>
        /// Get detail of a Story.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac.28076/">Story's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(StoryDetail), 200)]
        [HttpGet]
        [Route("{serverIndex}/{storyId}")]
        public IActionResult GetStoryDetail(int serverIndex, string storyId)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            var crawler = StorySourceScanner.Instance.Commands[serverIndex];
            return Ok(crawler.GetStoryDetail(storyId));
        }

        /// <summary>
        /// Get all chapters of a Story.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="storyId" example="khuyet-tu-tam-sa.38965/">Story's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(Chapter[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/{storyId}/chapters/all")]
        public IActionResult GetAllChaptersOfStory(int serverIndex, string storyId)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            var crawler = StorySourceScanner.Instance.Commands[serverIndex];
            return Ok(crawler.GetChaptersOfStory(storyId));
        }

        /// <summary>
        /// Get chapters of a Story with paging.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="storyId" example="khuyet-tu-tam-sa.38965/">Story's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<Chapter>), 200)]
        [HttpGet]
        [Route("{serverIndex}/{storyId}/chapters")]
        public IActionResult GetChaptersOfStory(int serverIndex,string storyId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            var crawler = StorySourceScanner.Instance.Commands[serverIndex];
            return Ok(crawler.GetChaptersOfStory(storyId, page, limit));
        }

        /// <summary>
        /// Get Chapter content from a Story.
        /// </summary>
        /// <param name="serverIndex">Index of the server to check.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac.28076/">Story's identity of each page, usally the last section of URL.</param>
        /// <param name="chapterIndex" example="1">Index of chapter (starts from 1).</param>
        [ProducesResponseType(typeof(ChapterContent), 200)]
        [HttpGet]
        [Route("{serverIndex}/{storyId}/chapter")]
        public IActionResult GetChapterContent(int serverIndex, string storyId, [FromQuery(Name = "index")] int chapterIndex)
        {
            bool isValid = Handler.ServerHandler.CheckServerIndex(serverIndex);
            if (!isValid) return BadRequest("Invalid server index.");
            return Ok(StorySourceScanner.Instance.Commands[serverIndex].GetChapterContent(storyId, chapterIndex));
        }
    }
}
