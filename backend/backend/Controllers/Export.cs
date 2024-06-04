using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;
using backend.DLLScanner;
using System.Net.Mime;
using PluginBase.Contract;
namespace backend.Controllers
{
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
                var chapterContents = AsyncGetAllChapterContents(command, storyID);

                // why async here?
                byte[] bytes = await Task.Run(() => ScannerController.Instance.exporterScanner.Commands[type].ExportStory(storyDetail, chapterContents));

                return File(bytes, "application/octet-stream", $"{storyID}.{ScannerController.Instance.exporterScanner.Commands[type].Ext}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get download link: {e.Message}.");
            }
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