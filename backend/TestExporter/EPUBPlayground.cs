using ExporterEPUB;
using ExporterEPUB.FilePrepare;
using ExporterEPUB.Helpers;
using ExporterEPUB.XHTMLBuilder;
using PluginBase.Models;
using System.Security;
using TangThuVien;

namespace TestExporter;

public class EPUBPlayground
{
    public static void Test()
    {
        var e = new EPUB();
    }

    // async version
    public static byte[] Test_CreateXHTML()
    {



        var path = FileGenerator.SaveImage(sd.ImageUrl!);
        var ex = new AllGenerator(sd, res.ToList(), path);
        ex.CreareFile();
        return [];
    }

    // sync version
    public static void Test_CreateXHTML_Sync()
    {
        var t = new TangThuVienCrawler();
        var chapterContents = new List<ChapterContent>();
        var sd = t.GetStoryDetail("thi-ra-ho-moi-la-nhan-vat-chinh");
        var chapters = t.GetChaptersOfStory("thi-ra-ho-moi-la-nhan-vat-chinh");
        foreach (var chapter in chapters)
        {
            var content = t.GetChapterContent(chapter.Id);
            content.Chapter = chapter;
            chapterContents.Add(content);
        }
        var rg = new FileGenerator();
        var path = rg.SaveImage(sd.ImageUrl!);
        var ex = new AllGenerator(sd, chapterContents, path);
        ex.CreareFile();
    }
}
