using backend.StorySourcesScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class TestController : Controller
    {
        DLLScanner dLLScanner;
        public TestController() 
        {
            dLLScanner = DLLScanner.Instance;
            dLLScanner.startScanThread();
        }

        [HttpGet]
        [Route("api/yourroute")]
        public ActionResult GetNumber()
        {
            if (this.dLLScanner.commands.Count == 0)
            {
                return NotFound();
            }

            var res = this.dLLScanner.commands[0].GetCategories();

            return Ok(res);
        }
    }
}
