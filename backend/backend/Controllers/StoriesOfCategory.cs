using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/category")]
    public class StoriesOfCategory : Controller
    {
        [HttpGet]
        [Route("{category}")]
        public IActionResult Index(string category)
        {
            string shit = category;
            return Ok(StorySourceScanner.Instance.commands[0].GetStoryInfoOfCategory(category));
        }
    }
}
