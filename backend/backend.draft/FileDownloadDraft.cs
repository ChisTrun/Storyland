using TangThuVien;
using backend.Model;

namespace backend.draft;

public class FileDownloadDraft
{
    public static void Test1()
    {
        var ttv = new TangThuVienCrawler();
        var file = PluginBase.Repositories.ChapterRepo.AsyncGetAllChapterContents(ttv, "dichnguyen-thuy-dai-thien-ton-suu-tam");
    }
}
