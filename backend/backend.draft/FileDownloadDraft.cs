using TangThuVien;
using backend.Model;

namespace backend.draft;

public class FileDownloadDraft
{
    public static void Test1()
    {
        //var ttv = new TangThuVienCrawler();
        var tf = new TruyenFullPlugin.TruyenFullCommand();
        //var file = PluginBase.Repositories.ChapterRepo.AsyncGetAllChapterContents(ttv, "dichnguyen-thuy-dai-thien-ton-suu-tam");
        var file = PluginBase.Repositories.ChapterRepo.GetAllChapterContents(tf, "xem-nhu-anh-loi-hai-do-xau-xa");        
        Console.WriteLine(file);
    }
}
