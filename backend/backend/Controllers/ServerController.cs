using Backend.Application.DTO;
using Backend.Application.Services.Abstract;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Controllers;

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
            return StatusCode(500, $"Failed to get servers: {e.Message}.");
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
            return StatusCode(500, $"Failed to change status: {e.Message}.");
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
            return StatusCode(500, $"Failed to upload files: {e.Message}.");
        }
    }

    /// <summary>
    /// Get list of all IDs.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("ids")]
    [ProducesResponseType(typeof(string[]), 200)]
    public IActionResult All()
    {
        try
        {
            return Ok(_pluginsScannerService.GetCrawlerPluginInfos().Select(x => x.ID));
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Failed to get IDs: {e.Message}.");
        }
    }
}
