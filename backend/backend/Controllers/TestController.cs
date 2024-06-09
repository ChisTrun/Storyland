using backend.DLLScanner.Utilis;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("tôi là phú xuân");
        }
    }
}
