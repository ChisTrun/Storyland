using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/story")]
    public class Story : Controller
    {
        [HttpGet]
        [Route("{storyName}")]
        public IActionResult GetStoryDetail(string storyName)
        {
            var crawler = StorySourceScanner.Instance.Commands[0];
            return Ok(crawler.GetStoryDetail(storyName));
        }

        [HttpGet]
        [Route("{storyName}/chapters")]
        public IActionResult GetAllChaptersOfStory(string storyName)
        {
            var crawler = StorySourceScanner.Instance.Commands[0];
            return Ok(crawler.GetChaptersOfStory(storyName));
        }

        [HttpGet]
        [Route("{storyName}/chapter")]
        public IActionResult GetChapterContent(string storyName, [FromQuery(Name = "index")] int chapterIndex)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetChapterContent(storyName, chapterIndex));
        }
    }
}
