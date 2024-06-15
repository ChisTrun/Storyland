using Backend.Application.DTO;
using Backend.Application.Services.Abstract;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/export")]
public class ExportController : Controller
{
    private readonly IPluginsScannerService _pluginsScannerService;
    private readonly IExportService _exportService;

    public ExportController(IPluginsScannerService pluginsScannerService, IExportService exportService)
    {
        _pluginsScannerService = pluginsScannerService;
        _exportService = exportService;
    }

    /// <summary>
    /// Get all export formats.
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(PluginInfoDTO[]), 200)]
    [HttpGet]
    public IActionResult GetExportFormats()
    {
        try
        {
            return Ok(_pluginsScannerService.GetExporterPluginInfos());
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Fail to get export formats: {e.Message}.");
        }
    }

    /// <summary>
    /// Export file with format.
    /// </summary>
    /// <param name="serverId">Server choosen to get story.</param>
    /// <param name="fileTypeId">File format choosen.</param>
    /// <param name="storyId" example="nhat-tich-dac-dao">Story's ID to get.</param>
    /// <returns></returns>
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [HttpGet]
    [Route("{serverId}/{fileTypeId}/{storyId}/")]
    public IActionResult ExportFile(string serverId, string fileTypeId, string storyId)
    {
        try
        {
            var file = _exportService.CreateFile(serverId, fileTypeId, storyId);
            return File(file.Bytes, "application/octet-stream", $"{storyId}.{file.Extension}");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Fail to get download link: {e.Message}.");
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
            _pluginsScannerService.GetExporterScanner().ChangeStatus(serverID);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Fail to change status: {e.Message}.");
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
            var message = PluginFile.UploadFiles(_pluginsScannerService.GetExporterScanner(), files);
            return Ok(new { message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
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
            return Ok(_pluginsScannerService.GetExporterPluginInfos().Select(x => x.ID));
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Fail to get IDs: {e.Message}.");
        }
    }
}