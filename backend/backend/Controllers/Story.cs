using backend.DLLScanner;
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
        /// <param name="storyId" example="/dao-gia-muon-phi-thang-dao-gia-yeu-phi-thang">Story's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(StoryDetail), 200)]
        [HttpGet]
        [Route("{storyId}")]
        public IActionResult GetStoryDetail(string storyId)
        {
            var crawler = StorySourceScanner.Instance.Commands[0];
            return Ok(crawler.GetStoryDetail(storyId));
        }

        /// <summary>
        /// Get all chapters of a Story.
        /// </summary>
        /// <param name="storyId" example="/dao-gia-muon-phi-thang-dao-gia-yeu-phi-thang">Story's identity of each page, usally the last section of URL.</param>
        [ProducesResponseType(typeof(Chapter[]), 200)]
        [HttpGet]
        [Route("{storyId}/chapters/all")]
        public IActionResult GetAllChaptersOfStory(string storyId)
        {
            var crawler = StorySourceScanner.Instance.Commands[0];
            return Ok(crawler.GetChaptersOfStory(storyId));
        }

        /// <summary>
        /// Get chapters of a Story with paging.
        /// </summary>
        /// <param name="storyId" example="/dao-gia-muon-phi-thang-dao-gia-yeu-phi-thang">Story's identity of each page, usally the last section of URL.</param>
        /// <param name="page" example="2">Current page (starts from 1).</param>
        /// <param name="limit" example="5">Records per page.</param>
        [ProducesResponseType(typeof(PagingRepresentative<Chapter>), 200)]
        [HttpGet]
        [Route("{storyId}/chapters")]
        public IActionResult GetChaptersOfStory(string storyId, [FromQuery(Name = "page")] int page, [FromQuery(Name = "limit")] int limit)
        {
            var crawler = StorySourceScanner.Instance.Commands[0];
            return Ok(crawler.GetChaptersOfStory(storyId, page, limit));
        }

        /// <summary>
        /// Get Chapter content from a Story.
        /// </summary>
        /// <param name="storyId" example="/dao-gia-muon-phi-thang-dao-gia-yeu-phi-thang">Story's identity of each page, usally the last section of URL.</param>
        /// <param name="chapterIndex" example="1">Index of chapter (starts from 1).</param>
        [ProducesResponseType(typeof(ChapterContent), 200)]
        [HttpGet]
        [Route("{storyId}/chapter")]
        public IActionResult GetChapterContent(string storyId, [FromQuery(Name = "index")] int chapterIndex)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetChapterContent(storyId, chapterIndex));
        }
    }
}
