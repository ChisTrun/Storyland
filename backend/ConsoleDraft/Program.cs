using PluginBase.Contract;
using PluginBase.Models;
using System.Text;
using TangThuVienHttp;


namespace ConsoleDraft;

class Program
{
    static void Main(string[] args)
    {
        MainTest();
    }

    static void MainTest()
    {
        Console.OutputEncoding = Encoding.Unicode;

        var t = new TangThuVienHttpCrawler();
        Test(t);
    }

    static void Test(ICrawler crawler)
    {
        Console.OutputEncoding = Encoding.Unicode;

        var categories = crawler.GetCategories();
        foreach (var category in categories)
        {
            Console.WriteLine(category.Url);
            Console.WriteLine(category.Name);
        }

        var storiesCategory = crawler.GetStoriesOfCategory("Tiên Hiệp");
        foreach (var story in storiesCategory)
        {
            Console.WriteLine(story.Name);
            Console.WriteLine(story.Url);
        }

        IEnumerable<Story> stories = crawler.GetStoriesBySearchName("Đỉnh");
        foreach (var story in stories)
        {
            Console.WriteLine(story.Name);
            Console.WriteLine(story.Url);
        }

        return;

        var content1 = crawler.GetChapterContent("https://truyen.tangthuvien.vn/doc-truyen/ta-von-khong-y-thanh-tien/chuong-1");
        var content2 = crawler.GetChapterContent("https://truyen.tangthuvien.vn/doc-truyen/ta-von-khong-y-thanh-tien/chuong-0");
        Console.WriteLine(content1.PreChapUrl);
        Console.WriteLine(content1.NextChapUrl);
        Console.WriteLine(content2.PreChapUrl);
        Console.WriteLine(content2.NextChapUrl);

        var chapterInfos = crawler.GetChaptersOfStory("https://truyen.tangthuvien.vn/doc-truyen/gia-toc-tu-tien-tong-thi-truong-thanh");
        foreach (var chapterInfo in chapterInfos)
        {
            Console.WriteLine(chapterInfo.Name);
            Console.WriteLine(chapterInfo.Url);
        }

        //IEnumerable<Author> authorInfos = t.GetAuthorsBySearchName("Đỉnh");
        //foreach (var authorInfo in authorInfos)
        //{
        //    Console.WriteLine(authorInfo.Name);
        //    Console.WriteLine(authorInfo.Url);
        //}

        var storyInfos_Author = crawler.GetStoriesOfAuthor("https://truyen.tangthuvien.vn/tac-gia?author=65");
        foreach (var storyInfo in storyInfos_Author)
        {
            Console.WriteLine(storyInfo.Name);
            Console.WriteLine(storyInfo.Url);
        }

        //var storyCategoriesPage = t.GetStoryInfoOfCategoryByPage("https://truyen.tangthuvien.vn/tong-hop?ctg=1", 1699, 2);
        //foreach (var story in storyCategoriesPage)
        //{
        //    Console.WriteLine(story.Name);
        //    Console.WriteLine(story.Url);
        //}
    }
}
