using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;
using backend.DLLScanner;
using System.Net.Mime;
using PluginBase.Contract;
using System.Reflection;
namespace backend.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class Export : Controller
    {
        /// <summary>
        /// Get export types
        /// </summary>
        /// <param name="" example =""></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ExportType[]), 200)]
        [HttpGet]
        public IActionResult GetExportFormats()
        {
            try
            {
                var list = ExporterScanner.Instance.Commands.Where(souce => backend.Handler.ExporterHandler.CheckExporterID(souce.Key)).Select(exporter => new ExportType(exporter.Key , exporter.Value.Item1.Ext));
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
        /// <param name="serverID" example ="1"></param>
        /// <param name="type" example="1"></param>
        /// <param name="storyID" example ="hop-dong-ba-nam-yeu-duong"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Category[]), 200)]
        [HttpGet]
        [Route("{serverID}/{type}/{storyID}/")]
        public async Task<IActionResult> GetAllCategories(string serverID, string type, string storyID)
        {
            try
            {
                var command = StorySourceScanner.Instance.Commands[serverID];
                var storyDetail = command.Item1.GetStoryDetail(storyID);
                var chapterContents = AsyncGetAllChapterContents(command.Item1, storyID);

                // why async here?
                byte[] bytes = await Task.Run(() => ScannerController.Instance.exporterScanner.Commands[type].Item1.ExportStory(storyDetail, chapterContents));

                return File(bytes, "application/octet-stream", $"{storyID}.{ScannerController.Instance.exporterScanner.Commands[type].Item1.Ext}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get download link: {e.Message}.");
            }
        }

        [HttpPost("changestatus")]
        public IActionResult ChangeStatus([FromBody] string serverID)
        {
            ScannerController.Instance.exporterScanner.ChangeStatus(serverID);
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
                    var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ScannerController.Instance.exporterScanner.PluginsFolder, formFile.FileName);

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
            ScannerController.Instance.exporterScanner.ScanDLLFiles();
            return Ok(new { message });
        }

    private static List<ChapterContent> GetAllChapterContents(ICrawler command, string storyId)
        {
            var chapters = command.GetChaptersOfStory(storyId).ToList();
            var chapterContents = new List<ChapterContent>();
            foreach (var chapter in chapters)
            {
                var chapterContent = command.GetChapterContent(storyId, chapter.Index + 1);
                chapterContents.Add(chapterContent);
            }
            return chapterContents;
        }

        private static List<ChapterContent> AsyncGetAllChapterContents(ICrawler command, string storyId)
        {
            var storyDetail = command.GetStoryDetail(storyId);
            var chapters = command.GetChaptersOfStory(storyId);
            List<Task<ChapterContent>> tasks = new();
            foreach (var chapter in chapters)
            {
                tasks.Add(Task.Run(() =>
                {
                    var content = command.GetChapterContent(storyId, chapter.Index);
                    content.ChapterName = chapter.Name;
                    content.ChapterID = chapter.Id;
                    content.ChapterIndex = chapter.Index;
                    return content;
                }));
            }
            var chapterContents = Task.WhenAll(tasks).Result.ToList();
            return chapterContents;
        }
    }
}