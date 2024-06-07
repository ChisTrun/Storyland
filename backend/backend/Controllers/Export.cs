using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;
using backend.DLLScanner;
using System.Net.Mime;
using PluginBase.Contract;
using backend.Model;
namespace backend.Controllers
{
    [Route("api/export")]
    public class Export : Controller
    {
        /// <summary>
        /// Get export types
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ExportType[]), 200)]
        [HttpGet]
        public IActionResult GetExportFormats()
        {
            try
            {
                var list = ExporterScanner.Instance.Commands.Select((com, index) => new ExportType(index, com.Ext));
                return Ok(list);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get export formats: {e.Message}.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIndex" example ="1"></param>
        /// <param name="type" example="1"></param>
        /// <param name="storyID" example ="hop-dong-ba-nam-yeu-duong"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Category[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/{type}/{storyID}/")]
        public async Task<IActionResult> GetAllCategories(int serverIndex, int type, string storyID)
        {
            try
            {
                var command = StorySourceScanner.Instance.Commands[serverIndex];
                var storyDetail = command.GetStoryDetail(storyID);
                var chapterContents = CrawlerModel.AsyncGetAllChapterContents(command, storyID);

                // why async here?
                byte[] bytes = await Task.Run(() => ScannerController.Instance.exporterScanner.Commands[type].ExportStory(storyDetail, chapterContents));

                return File(bytes, "application/octet-stream", $"{storyID}.{ScannerController.Instance.exporterScanner.Commands[type].Ext}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get download link: {e.Message}.");
            }
        }
    }
}