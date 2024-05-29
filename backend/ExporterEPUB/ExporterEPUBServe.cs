using ExporterEPUB.FilePrepare;
using ExporterEPUB.Helpers;
using PluginBase.Models;

namespace ExporterEPUB
{
    public class ExporterEPUBServe : IDisposable
    {
        private readonly FolderStructure _structure;
        private readonly StoryDetail _storyDetail;
        private readonly List<ChapterContent> _chapterContents;

        public ExporterEPUBServe(StoryDetail storyDetail, List<ChapterContent> chapterContents)
        {
            _storyDetail = storyDetail;
            _chapterContents = chapterContents;
            var gui = $"Epub-{Guid.NewGuid()}-{DateTime.Now:dd-MM-yyyy}";
            var dir = Directory.CreateDirectory(gui);
            _structure = new FolderStructure(dir.FullName, FolderStructure.EPUBRESOURCE);
        }

        public byte[] ExportEpub()
        {
            var imgPath = FileGenerator.SaveImage(_storyDetail.ImageUrl, _structure);
            var allGen = new AllGenerator(_storyDetail, _chapterContents, imgPath, _structure);
            allGen.CreareFile();
            var zipFileBytes = FileZipper.Zip(_structure.BaseAbsoluteDir);
            return zipFileBytes;
        }

        public void Dispose()
        {
            Directory.Delete(_structure.BaseAbsoluteDir, true);
        }
    }
}

//var t = new TangThuVienCrawler();
//var chapterContents = new List<ChapterContent>();
//var s1 = "thi-ra-ho-moi-la-nhan-vat-chinh";
//var s2 = "nga-de-duy-the-gioi-duy-nhat-chan-than";
//var s = s1;
//var sd = t.GetStoryDetail(s);
//var chapters = t.GetChaptersOfStory(s);
//List<Task<ChapterContent>> tasks = new();
//foreach (var chapter in chapters)
//{
//    tasks.Add(Task.Run(() =>
//    {
//        var temp = new TangThuVienCrawler();
//        var content = temp.GetChapterContent(chapter.Id);
//        content.Chapter = chapter;
//        return content;
//    }));
//}

//var res = Task.WhenAll(tasks).Result;
//FolderStructure.Generate();
//var rg = new FileGenerator();
//var path = rg.SaveImage(sd.ImageUrl!);
//var ex = new AllGenerator(sd, res.ToList(), path);
//ex.CreareFile();
//return FileZipper.Zip();
