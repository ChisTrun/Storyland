using backend.draft;
using PluginBase.Models;
using System.Text;
using TangThuVien;
Console.OutputEncoding = Encoding.Unicode;

// For writing tests
//Console.OutputEncoding = Encoding.Unicode;

var ttv = new TangThuVienCrawler();
//TangThuVienSet.TestSet1(ttv);
//TangThuVienDraft.TestSet2(ttv);
FileDownloadDraft.Test1();
