using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using PluginBase.Exceptions;
using PluginBase.Models;
using PluginBase.Utils;
using System.Text;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
Console.OutputEncoding = Encoding.Unicode;

// For writing tests
//Console.OutputEncoding = Encoding.Unicode;

//var ttv = new TangThuVienCrawler();
//TangThuVienSet.TestSet1(ttv);
//TangThuVienDraft.TestSet2(ttv);

//FileDownloadDraft.Test1();