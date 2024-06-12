using backend.Application.DTO;
using backend.Application.Services.Abstract;
using backend.Domain.Objects;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/story")]
    public class StoryController : Controller
    {
        private readonly ICrawlingService _crawlingService;

        public StoryController(ICrawlingService crawlingService)
        {
            _crawlingService = crawlingService;
        }

        /// <summary>
        /// Get detail of a Story.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac">Story's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(StoryDetailDTO), 200)]
        [HttpGet]
        [Route("{serverId}/{storyId}")]
        public IActionResult GetStoryDetail(string serverId, string storyId)
        {
            try
            {
                return Ok(_crawlingService.GetStoryDetail(serverId, storyId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get detail of the story with id {storyId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get all chapters of a Story.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac">Story's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(ChapterDTO[]), 200)]
        [HttpGet]
        [Route("{serverId}/{storyId}/chapters/all")]
        public IActionResult GetChaptersOfStory(string serverId, string storyId)
        {
            try
            {
                return Ok(_crawlingService.GetChaptersOfStory(serverId, storyId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get chapters list of the story with id {storyId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get chapters of a Story with paging.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac">Story's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagedList<ChapterDTO>), 200)]
        [HttpGet]
        [Route("{serverId}/{storyId}/chapters")]
        public IActionResult GetChaptersOfStory(string serverId, string storyId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            try
            {
                return Ok(_crawlingService.GetChaptersOfStory(serverId, storyId, page, limit));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get chapters list at page {page} of the story with id {storyId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get number of chapters of a story.
        /// </summary>
        /// <param name="serverId">ID of server.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac">Story's identity of each page, usally the last section of URL.</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(int), 200)]
        [HttpGet]
        [Route("{serverId}/{storyId}/chapters/count")]
        public IActionResult CountAllChaptersOfStory(string serverId, string storyId)
        {
            try
            {
                return Ok(_crawlingService.GetChaptersCount(serverId, storyId));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get chapters list of the story with id {storyId}: {e.Message}.");
            }
        }

        /// <summary>
        /// Get Chapter content from a Story via storyId + index.
        /// </summary>
        /// <param name="serverId">ID of the server.</param>
        /// <param name="storyId" example="bat-lo-thanh-sac">Story's identity of each page, usally the last section of URL.</param>
        /// <param name="chapterIndex" example="0">Index of chapter (starts from 0).</param>
        [ProducesResponseType(typeof(ChapterContentDTO), 200)]
        [HttpGet]
        [Route("{serverId}/story/chapter")]
        public IActionResult GetChapterContent(string serverId, [FromQuery(Name = "storyid")] string storyId, [FromQuery(Name = "index")] int chapterIndex)
        {
            try
            {
                return Ok(_crawlingService.GetChapterContent(serverId, storyId, chapterIndex));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get content of the chapter index {chapterIndex} of the story with id {storyId}: {e.Message}.");
            }
        }
    }
}
