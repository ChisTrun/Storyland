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
        var GetCategoriesPass = true;
        var GetStoriesOfCategoryPass = true;
        var GetStoriesBySearchNamePass = true;
        var GetStoriesOfAuthorPass = true;
        var GetChaptersOfStoryPass = true;
        var GetChapterContentPass = true;
        var GetStoryDetail = true;
        //GetCategoriesPass = false;
        //GetStoriesOfCategoryPass = false;
        //GetStoriesBySearchNamePass = false;
        //GetStoriesOfAuthorPass = false;
        //GetChaptersOfStoryPass = false;
        //GetChapterContentPass = false;
        GetStoryDetail = false;

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

        if (GetStoryDetail == false)
        {
            var storyDetail = crawler.GetStoryDetail("Thì Ra, Họ Mới Là Nhân Vật Chính (Nguyên Lai Tha Môn Tài Thị Chủ Giác?)");
            Console.WriteLine(storyDetail.Name);
            Console.WriteLine(storyDetail.Url);
            Console.WriteLine(storyDetail.ImageUrl);
            Console.WriteLine(storyDetail.Author.Name);
            Console.WriteLine(storyDetail.Author.Url);
            Console.WriteLine(storyDetail.Status);
            Console.WriteLine(storyDetail.Categories.Length);
            Console.WriteLine(storyDetail.Categories[0].Name);
            Console.WriteLine(storyDetail.Categories[0].Url);
            Console.WriteLine(storyDetail.Description);
        }
    }
}
