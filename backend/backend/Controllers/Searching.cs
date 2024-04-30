using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/search")]
    public class Searching : Controller
    {
        [HttpGet]
        [Route("tacgia/{author}")]
        public IActionResult SearchByAuthor(string author)
        {
            return Ok(StorySourceScanner.Instance.commands[0].GetStoriesFromSearchingExactAuthor(author));
        }

        [HttpGet]
        [Route("truyen/{storyName}")]
        public IActionResult SearchByStoryName(string storyName)
        {
            return Ok(StorySourceScanner.Instance.commands[0].GetStoriesFromSearchingName(storyName));
        }
    }
}
