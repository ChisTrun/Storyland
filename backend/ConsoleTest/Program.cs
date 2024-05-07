using ConsoleTest;
using System.Text;
using TangThuVienPlugin;
using TruyenFullPlugin;

Console.OutputEncoding = Encoding.Unicode;

var ttv = new TangThuVienCrawler();
ttv.GetChapterContent("https://truyen.tangthuvien.vn/doc-truyen/ta-von-khong-y-thanh-tien/chuong-0");
