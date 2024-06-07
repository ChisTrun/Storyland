using backend.DLLScanner.Concrete;
using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;

namespace backend.Controllers
{
    [Route("api/story")]
    public class Story : Controller
    {
        /// <summary>
        /// Get detail of a Story.
        /// </summary>
        /// <param name="serverID" example="0">ID of the server.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac/">Story's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(StoryDetail), 200)]
        [HttpGet]
        [Route("{serverID}/{storyId}")]
        public IActionResult GetStoryDetail(string serverID, string storyId)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerID(serverID);
                if (!isValid)
                    return BadRequest("Invalid server index.");
                var crawler = StorySourceScanner.Instance.Commands[serverID];
                return Ok(crawler.GetStoryDetail(storyId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get detail of the story with id {storyId}: {e.Message}.");
            }
            
        }

        /// <summary>
        /// Get all chapters of a Story.
        /// </summary>
        /// <param name="serverID" example="0">ID of the server.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac">Story's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(Chapter[]), 200)]
        [HttpGet]
        [Route("{serverID}/{storyId}/chapters/all")]
        public IActionResult GetAllChaptersOfStory(string serverID, string storyId)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerID(serverID);
                if (!isValid)
                    return BadRequest("Invalid server index.");
                var crawler = StorySourceScanner.Instance.Commands[serverID];
                return Ok(crawler.GetChaptersOfStory(storyId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get chapters list of the story with id {storyId}: {e.Message}.");
            }
            
        }

        /// <summary>
        /// Get chapters of a Story with paging.
        /// </summary>
        /// <param name="serverID">ID of the server to check.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac/">Story's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<Chapter>), 200)]
        [HttpGet]
        [Route("{serverID}/{storyId}/chapters")]
        public IActionResult GetChaptersOfStory(string serverID, string storyId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerID(serverID);
                if (!isValid)
                    return BadRequest("Invalid server index.");
                var crawler = StorySourceScanner.Instance.Commands[serverID];
                return Ok(crawler.GetChaptersOfStory(storyId, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get chapters list at page {page} of the story with id {storyId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get Chapter content from a Story via storyId + index
        /// </summary>
        /// <param name="serverID">ID of the server to check.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac">Story's identity of each page, usally the last section of URL.</param>
        /// <param name="chapterIndex" example="1">ID of chapter (starts from 0).</param>
        [ProducesResponseType(typeof(ChapterContent), 200)]
        [HttpGet]
        [Route("{serverID}/story/chapter")]
        public IActionResult GetChapterContent(string serverID, [FromQuery(Name = "storyid")] string storyId, [FromQuery(Name = "index")] int chapterIndex)
        {
            try
            {
                bool isValid = Handler.ServerHandler.CheckServerID(serverID);
                if (!isValid)
                    return BadRequest("Invalid server index.");
                return Ok(StorySourceScanner.Instance.Commands[serverID].GetChapterContent(storyId, chapterIndex));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get content of the chapter index {chapterIndex} of the story with id {storyId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get Chapter content from a Story via chapterID
        /// </summary>
        /// <param name="serverID">ID of the server to check.</param>
        /// <param name="chapterId" example="trong-sinh-chi-vu-em-nhan-nha-sinh-hoat/chuong-480">Chapter's identity of each page, usally the last section of URL.</param>
        /// [ProducesResponseType(typeof(ChapterContent), 200)]
        /// [HttpGet]
        /// [Route("{serverID}/story/chapter/id")]
        /// public IActionResult GetChapterContent(int serverID, [FromQuery(Name = "chapterid")] string chapterId)
        /// {
        ///     try
        ///     {
        ///         bool isValid = Handler.ServerHandler.CheckServerID(serverID);
        ///         if (!isValid)
        ///             return BadRequest("Invalid server index.");
        ///         return Ok(StorySourceScanner.Instance.Commands[serverID].GetChapterContent(chapterId));
        ///     }
        ///     catch (Exception e)
        ///     {
        ///         return StatusCode(500, $"Fail to get content of the chapter with id {chapterId}: {e.Message}.");
        ///     }   
        /// }
    }
}
