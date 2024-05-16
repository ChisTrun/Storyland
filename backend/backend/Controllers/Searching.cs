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
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesOfAuthor(author));
        }

        [HttpGet]
        [Route("truyen/{storyName}")]
        public IActionResult SearchByStoryName(string storyName)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesBySearchName(storyName));
        }
    }
}
