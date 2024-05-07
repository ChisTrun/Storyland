using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/story")]
    public class Story : Controller
    {
        [HttpGet]
        [Route("{storyURL}")]
        public IActionResult GetAllChaptersOfStory(string storyURL)
        {
            return Ok(StorySourceScanner.Instance.commands[0].GetChaptersOfStory(storyURL));
        }

        [HttpGet]
        [Route("chapter/{chapterURL}")]
        public IActionResult GetAllChapterContent(string chapterURL)
        {
            return Ok(StorySourceScanner.Instance.commands[0].GetChapterContent(chapterURL));
        }
    }
}
