using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Test
{
    public class TestController : Controller
    {
        private readonly StorySourceScanner _storySourceScanner;
        public TestController()
        {
            _storySourceScanner = StorySourceScanner.Instance;
        }

        [HttpGet]
        [Route("api/test")]
        public ActionResult GetNumber()
        {
            if (_storySourceScanner.commands.Count == 0)
            {
                return NotFound();
            }

            var res = _storySourceScanner.commands[0].GetCategories();

            return Ok(res);
        }
    }
}
