using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;
using Microsoft.AspNetCore.Identity.Data;
using backend.DLLScanner;
using System.Reflection;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/server")]
    public class StorySourceInfo : Controller
    {
        /// <summary>
        /// Get servers.
        /// </summary>
        /// <remarks>        
        /// Get servers information with indexes.
        /// </remarks>
        [ProducesResponseType(typeof(Server[]), 200)]
        [HttpGet]
        public IActionResult GetServers()
        {
            try
            {
                var list = StorySourceScanner.Instance.Commands.Select(source => new Server(source.Key, source.Value.Name));
                return Ok(list);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get servers: {e.Message}.");
            }
        }

        [HttpPost("delete")]
        public IActionResult DeletePlugin([FromBody] string serverID)
        {
            ScannerController.Instance.sourceScanner.Commands.Remove(serverID);
            return Ok();
        }

        [HttpGet("useTF")]
        public IActionResult USETF()
        {
            AppDomain appDomain = AppDomain.CreateDomain("USETF");
            string assemblyPath = @"F:\Tai_lieu_dh\ThietKePhanMem\Project\Storyland\backend\backend\bin\Debug\net8.0\plugins\TruyenFullPlugin.dll";
            appDomain.Load(AssemblyName.GetAssemblyName(assemblyPath));

            
            return Ok();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPlugin([FromForm] FileUploadModel model)
        {
            var file = model.File;
            if (file == null || file.Length == 0)
                return BadRequest("Upload a file.");

            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ScannerController.Instance.sourceScanner.PluginsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Implement plugin loading logic
            return Redirect("/index");
        }

    }
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
    }
}
