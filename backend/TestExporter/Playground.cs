using ICSharpCode.SharpZipLib.Zip;
using System.IO.Compression;
using System.Text;
using TangThuVien;

namespace TestExporter;

public class Playground
{
    public static void Test()
    {
        {
            using var fs = new FileStream("test.html", FileMode.Create);
            using var sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine("<h1>An Ngo</h1>");
            sw.WriteLine("<h2>An Ngo</h2>");
            sw.WriteLine("<p>An Ngo<p>");
        }
        {
            string res = "null";
            {
                using var fs = new FileStream("test.html", FileMode.Open);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                res = sr.ReadToEnd();
            }
            if (File.Exists(@"test.html"))
            {
                Console.WriteLine("Deleted");
                File.Delete(@"test.html");
            }
            Console.WriteLine(res);
        }
    }

    public static void Test_DownImage()
    {
        using (var client = new HttpClient())
        {
            using (var s = client.GetStreamAsync("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRl5YT6lt_nEz5XX7UzNo9mhO2_jIRhM0dfAA&s"))
            {
                using (var fs = new FileStream("localfile.jpg", FileMode.OpenOrCreate))
                {
                    s.Result.CopyTo(fs);
                }
            }
        }
    }

    public static void Test_GetStoryContent()
    {
        var t = new TangThuVienCrawler();
        var content = t.GetChapterContent("nhat-tich-dac-dao/chuong-2");
        Console.WriteLine(content.Content);
    }

    public static void Test_Zip()
    {
        FastZip z = new()
        {
            CreateEmptyDirectories = true
        };
        z.CreateZip("test.epub ", "Structure", true, "");
    }
}
