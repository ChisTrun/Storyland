using Microsoft.AspNetCore.Mvc;
using PluginBase.Models;
using backend.DLLScanner.Concrete;
using backend.DLLScanner;
namespace backend.Controllers
{
    [Route("api/export")]
    public class Export : Controller
    {
        ///// <summary>
        /////     
        ///// </summary>
        ///// <param name="type" example="pdf"></param>
        ///// <param name="storyID" example ="hop-dong-ba-nam-yeu-duong/"></param>
        ///// <returns></returns>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIndex" example ="1"></param>
        /// <param name="type" example="1"></param>
        /// <param name="storyID" example ="hop-dong-ba-nam-yeu-duong/"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Category[]), 200)]
        [HttpGet]
        [Route("{serverIndex}/{type}/{storyID}/")]
        public async Task<IActionResult> GetAllCategories(int serverIndex, int type, string storyID)
        {
            try
            {
                StoryDetail storyDetail = StorySourceScanner.Instance.Commands[serverIndex].GetStoryDetail(storyID);
                List<Chapter> chapters = StorySourceScanner.Instance.Commands[serverIndex].GetChaptersOfStory(storyID).ToList();
                List<ChapterContent> chapterContents = new List<ChapterContent>();

                foreach (var chapter in chapters)
                {
                    var chapterContent = StorySourceScanner.Instance.Commands[serverIndex].GetChapterContent(storyID, chapter.Index + 1);
                    chapterContents.Add(chapterContent);
                }

                byte[] bytes = await Task.Run(() => ScannerController.Instance.exporterScanner.Commands[type].ExportStory(storyDetail, chapterContents));

                return File(bytes, "application/octet-stream", $"story.{ScannerController.Instance.exporterScanner.Commands[type].Ext}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Fail to get download link: {e.Message}.");
            }
            
        }
    }
}