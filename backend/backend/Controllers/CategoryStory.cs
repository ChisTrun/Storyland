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
            return Ok(StorySourceScanner.Instance.Commands[0].GetCategories());
        }

        [HttpGet]
        [Route("{categoryName}")]
        public IActionResult GetAllStoriesOfCategory(string categoryName)
        {
            return Ok(StorySourceScanner.Instance.Commands[0].GetStoriesOfCategory(categoryName));
        }
    }
}
