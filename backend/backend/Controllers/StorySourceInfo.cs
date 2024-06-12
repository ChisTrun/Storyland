using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;
using Microsoft.AspNetCore.Identity.Data;
using backend.DLLScanner;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;


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
                var list = StorySourceScanner.Instance.Commands.Where(souce => backend.Handler.ServerHandler.CheckServerID(souce.Key)).Select(source => new Server(source.Key, source.Value.Item1.Name));
                return Ok(list);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get servers: {e.Message}.");
            }
        }

        [HttpPost("changestatus")]
        public IActionResult ChangeStatus([FromBody] string serverID)
        {
            ScannerController.Instance.sourceScanner.ChangeStatus(serverID);
            return Ok();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPlugins(List<IFormFile> files)
        {
            string message = string.Empty;
            int numberSuccess = 0;
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ScannerController.Instance.sourceScanner.PluginsFolder, formFile.FileName);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        numberSuccess++;
                    }
                    catch (Exception ex)
                    {
                        message += ex.Message + "\n";
                    }
                }
            }
            message += $"\nLoaded {numberSuccess} file(s).";
            ScannerController.Instance.sourceScanner.ScanDLLFiles();
            return Ok(new { message });
        }
    }
}
