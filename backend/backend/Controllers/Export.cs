using Microsoft.AspNetCore.Mvc;
using PluginBase.Contract;
using PluginBase.Models;
using PDFExporter;
using backend.DLLScanner;
using System.IO;
namespace backend.Controllers
{
    [Route("api/export")]
    public class Export : Controller
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="type" example="pdf"></param>
        /// <param name="storyID" example ="hop-dong-ba-nam-yeu-duong/"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Category[]), 200)]
        [HttpGet]
        [Route("{type}/{storyID}/")]
        public async Task<IActionResult> GetAllCategories(string type, string storyID)
        {
            //bool isValid = Handler.ServerHandler.CheckServerIndex(type);
            //if (!isValid) return BadRequest("Invalid server index.");
            //return Ok(StorySourceScanner.Instance.Commands[type].GetCategories());


            StoryDetail storyDetail = StorySourceScanner.Instance.Commands[1].GetStoryDetail(storyID);
            List<Chapter> chapters = StorySourceScanner.Instance.Commands[1].GetChaptersOfStory(storyID).ToList();
            List<ChapterContent> chapterContents = new List<ChapterContent>();

            foreach (var chapter in chapters)
            {
                var chapterContent = StorySourceScanner.Instance.Commands[1].GetChapterContent(storyID, chapter.Index + 1);
                chapterContents.Add(chapterContent);
            }

            IExporter exporter = new PDFExport();
            byte[] bytes = await Task.Run(() => exporter.ExportStory(storyDetail, chapterContents));

            return File(bytes, "application/octet-stream", "story.pdf");
        }
    }
}