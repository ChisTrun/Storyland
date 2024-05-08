using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/story")]
    public class Story : Controller
    {
        [HttpGet]
        [Route("{storyName}")]
        public IActionResult GetAllChaptersOfStory(string storyName)
        {
            var crawler = StorySourceScanner.Instance.commands[0];
            return Ok(crawler.GetChaptersOfStory(storyName));
        }

        [HttpGet]
        [Route("chapter/{storyName}")]
        public IActionResult GetAllChapterContent(string storyName, [FromQuery(Name = "index")] int chapterIndex)
        {
            return Ok(StorySourceScanner.Instance.commands[0].GetChapterContent(storyName, chapterIndex));
        }
    }
}
