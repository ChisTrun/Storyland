using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc;

namespace backend.Test
{
    [Route("api/test")]
    public class TestController : Controller
    {
        [HttpGet]
        public ActionResult GetNumber()
        { 
            return Ok("Cai con cac ne");
        }
    }
}
