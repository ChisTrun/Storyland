using Microsoft.AspNetCore.Mvc;
using backend.Application.Objects;
using backend.Application.Commands.Abstract;
using backend.Application.Services.Abstract;

namespace backend.Controllers;

[Route("api/export")]
public class ExportController : Controller
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    /// <summary>
    /// Get all export formats.
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(PluginInfo[]), 200)]
    [HttpGet]
    public IActionResult GetExportFormats()
    {
        try
        {
            return Ok(_exportService.GetExportFormats());
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
    [Route("{serverId}/{fileTypeId}/{storyID}/")]
    public IActionResult GetAllCategories(string serverId, string fileTypeId, string storyId)
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
}