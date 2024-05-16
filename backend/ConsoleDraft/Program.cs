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

        var ttv = new TangThuVienHttpCrawler();
        TestSet1(ttv);
    }

    static void TestSet1(TangThuVienHttpCrawler crawler)
    {
        Console.OutputEncoding = Encoding.Unicode;
        var GetCategoriesPass = false;
        var GetStoriesOfCategoryPass = false;
        var GetStoriesBySearchNamePass = false;
        var GetStoriesOfAuthorPass = false;
        var GetChaptersOfStoryPass = false;
        var GetChapterContentPass = false;

        if (GetCategoriesPass == false)
        {
            var categories = crawler.GetCategories();
            foreach (var category in categories)
            {
                Console.WriteLine(category.Url);
                Console.WriteLine(category.Name);
            }
        }

        if (GetStoriesOfCategoryPass == false)
        {
            var storiesCategory = crawler.GetStoriesOfCategory("Tiên Hiệp");
            foreach (var story in storiesCategory)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Url);
            }
        }

        if (GetStoriesBySearchNamePass == false)
        {
            IEnumerable<Story> stories = crawler.GetStoriesBySearchName("Đỉnh");
            foreach (var story in stories)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Url);
            }
        }

        if (GetStoriesOfAuthorPass == false)
        {
            var stories_Author = crawler.GetStoriesOfAuthor("Tối Bạch Đích Ô Nha");
            foreach (var story in stories_Author)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Url);
            }
        }

        if (GetChaptersOfStoryPass == false)
        {
            var chapters = crawler.GetChaptersOfStory("Thì Ra, Họ Mới Là Nhân Vật Chính (Nguyên Lai Tha Môn Tài Thị Chủ Giác?)");
            foreach (var chapter in chapters)
            {
                Console.WriteLine(chapter.Name);
                Console.WriteLine(chapter.Url);
            }
        }

        if (GetChapterContentPass == false)
        {
            //Console.WriteLine("Chương 1 : Vai Phụ Đúng Là Bản Thân Tôi" == "Chương 1 : Vai Phụ Đúng Là Bản Thân Tôi");
            var content1 = crawler.GetChapterContent("Thì Ra, Họ Mới Là Nhân Vật Chính (Nguyên Lai Tha Môn Tài Thị Chủ Giác?)", 0);
            var content2 = crawler.GetChapterContent("Thì Ra, Họ Mới Là Nhân Vật Chính (Nguyên Lai Tha Môn Tài Thị Chủ Giác?)", 1);
            Console.WriteLine(content1.Content);
            Console.WriteLine(content1.PreChapUrl);
            Console.WriteLine(content1.NextChapUrl);
            Console.WriteLine(content2.Content);
            Console.WriteLine(content2.PreChapUrl);
            Console.WriteLine(content2.NextChapUrl);
        }

        //IEnumerable<Author> authorInfos = t.GetAuthorsBySearchName("Đỉnh");
        //foreach (var authorInfo in authorInfos)
        //{
        //    Console.WriteLine(authorInfo.Name);
        //    Console.WriteLine(authorInfo.Url);
        //}

        //var storyCategoriesPage = t.GetStoryInfoOfCategoryByPage("https://truyen.tangthuvien.vn/tong-hop?ctg=1", 1699, 2);
        //foreach (var story in storyCategoriesPage)
        //{
        //    Console.WriteLine(story.Name);
        //    Console.WriteLine(story.Url);
        //}
    }
}
