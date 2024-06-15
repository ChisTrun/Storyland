using backend.Application.DTO;
using backend.Application.Services.Abstract;
using backend.Services;
using Microsoft.AspNetCore.Mvc;


namespace backend.Controllers;

[ApiController]
[Route("api/server")]
public class ServerController : Controller
{
    private readonly IPluginsScannerService _pluginsScannerService;

    public ServerController(IPluginsScannerService pluginsScannerService)
    {
        _pluginsScannerService = pluginsScannerService;
    }

    /// <summary>
    /// Get servers.
    /// </summary>
    [ProducesResponseType(typeof(PluginInfoDTO[]), 200)]
    [HttpGet]
    public IActionResult GetServers()
    {
        try
        {
            return Ok(_pluginsScannerService.GetCrawlerPluginInfos());
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Fail to get servers: {e.Message}.");
        }
    }

    /// <summary>
    /// Change plugins status (used / removed).
    /// </summary>
    /// <param name="serverID"></param>
    /// <returns></returns>
    [HttpPost("changestatus")]
    public IActionResult ChangeStatus([FromBody] string serverID)
    {
        try
        {
            _pluginsScannerService.GetCrawlerScanner().ChangeStatus(serverID);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Fail to get servers: {e.Message}.");
        }
    }

    /// <summary>
    /// Upload plugins files.
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    public IActionResult UploadPlugins(List<IFormFile> files)
    {
        try
        {
            var message = PluginFile.UploadFiles(_pluginsScannerService.GetCrawlerScanner(), files);
            return Ok(message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Fail to get servers: {e.Message}.");
        }
    }


    [HttpGet]
    [Route("a/test")]
    public IActionResult Test()
    {
        try
        {
            return Ok(_pluginsScannerService.GetCrawlerPluginInfos().Select(x => x.ID));
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Fail: {e.Message}.");
        }
    }
}
