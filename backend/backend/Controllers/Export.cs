using backend.DLLScanner;
using ExporterEPUB;
using Microsoft.AspNetCore.Mvc;
using PluginBase.Contract;
using PluginBase.Models;
using System.Net.Mime;
using TangThuVien;

namespace backend.Controllers
{
    [Route("api/export/test")]
    public class Export : Controller
    {
        [HttpGet]
        public IActionResult Test()
        {
            var command = StorySourceScanner.Instance.Commands[0];
            var chapterContents = new List<ChapterContent>();
            var s1 = "thi-ra-ho-moi-la-nhan-vat-chinh";
            var s2 = "nga-de-duy-the-gioi-duy-nhat-chan-than";
            var s = s2;
            var storyDetail = command.GetStoryDetail(s);
            var chapters = command.GetChaptersOfStory(s);
            List<Task<ChapterContent>> tasks = new();
            foreach (var chapter in chapters)
            {
                tasks.Add(Task.Run(() =>
                {
                    var temp = new TangThuVienCrawler();
                    var content = temp.GetChapterContent(chapter.Id);
                    content.Chapter = chapter;
                    return content;
                }));
            }
            var res = Task.WhenAll(tasks).Result;

            // Export
            var epub = new EPUBExport();
            var stream = epub.ExportStory(storyDetail, res.ToList());
            return File(stream, MediaTypeNames.Multipart.ByteRanges, "test.epub");
        }
    }
}
