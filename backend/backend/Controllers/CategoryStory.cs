using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/category")]
    public class CategoryStory : Controller
    {
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            return Ok(StorySourceScanner.Instance.commands[0].GetCategories());
        }

        [HttpGet]
        [Route("{categoryURL}")]
        public IActionResult GetAllStoriesOfCategory(string categoryURL)
        {
            return Ok(StorySourceScanner.Instance.commands[0].GetStoryInfoOfCategory(categoryURL));
        }
    }
}
