using PluginBase.Contract;
using PluginBase.Models;
using System.Text;
using TangThuVienHtmlCrawler;


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

        var ttv = new TangThuVienCrawler();
        TestSet1(ttv);
        TestSet2(ttv);
    }

    static void TestSet1(TangThuVienCrawler crawler)
    {
        var GetCategoriesPass = true;
        var GetStoriesOfCategoryPass = true;
        var GetStoriesBySearchNamePass = true;
        var GetStoriesOfAuthorPass = true;
        var GetChaptersOfStoryPass = true;
        var GetChapterContentPass = true;
        var GetStoryDetailPass = true;
        //GetCategoriesPass = false;
        //GetStoriesOfCategoryPass = false;
        //GetStoriesBySearchNamePass = false;
        //GetStoriesOfAuthorPass = false;
        //GetChaptersOfStoryPass = false;
        //GetChapterContentPass = false;
        //GetStoryDetailPass = false;

        if (GetCategoriesPass == false)
        {
            var categories = crawler.GetCategories();
            foreach (var category in categories)
            {
                Console.WriteLine(category.Id);
                Console.WriteLine(category.Name);
            }
        }

        if (GetStoriesOfCategoryPass == false)
        {
            var storiesCategory = crawler.GetStoriesOfCategory("?ctg=1");
            foreach (var story in storiesCategory)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Id);
            }
        }

        if (GetStoriesBySearchNamePass == false)
        {
            IEnumerable<Story> stories = crawler.GetStoriesBySearchName("Đỉnh");
            foreach (var story in stories)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Id);
            }
        }

        if (GetStoriesOfAuthorPass == false)
        {
            var stories_Author = crawler.GetStoriesOfAuthor("?author=27");
            foreach (var story in stories_Author)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Id);
            }
        }

        if (GetChaptersOfStoryPass == false)
        {
            var chapters = crawler.GetChaptersOfStory("/thi-ra-ho-moi-la-nhan-vat-chinh");
            foreach (var chapter in chapters)
            {
                Console.WriteLine(chapter.Name);
                Console.WriteLine(chapter.Id);
            }
        }

        if (GetChapterContentPass == false)
        {
            //Console.WriteLine("Chương 1 : Vai Phụ Đúng Là Bản Thân Tôi" == "Chương 1 : Vai Phụ Đúng Là Bản Thân Tôi");
            var content1 = crawler.GetChapterContent("/thi-ra-ho-moi-la-nhan-vat-chinh", 1);
            var content2 = crawler.GetChapterContent("/thi-ra-ho-moi-la-nhan-vat-chinh", 2);
            Console.WriteLine(content1.Content);
            Console.WriteLine(content1.PreChapUrl);
            Console.WriteLine(content1.NextChapUrl);
            Console.WriteLine(content2.Content);
            Console.WriteLine(content2.PreChapUrl);
            Console.WriteLine(content2.NextChapUrl);
        }
        // Anh Hùng Liên Minh Chi Thái Điểu Chi Quang

        if (GetStoryDetailPass == false)
        {
            var storyDetail = crawler.GetStoryDetail("/thi-ra-ho-moi-la-nhan-vat-chinh");
            Console.WriteLine(storyDetail.Name);
            Console.WriteLine(storyDetail.Id);
            Console.WriteLine(storyDetail.ImageUrl);
            Console.WriteLine(storyDetail.Author.Name);
            Console.WriteLine(storyDetail.Author.Id);
            Console.WriteLine(storyDetail.Status);
            Console.WriteLine(storyDetail.Categories.Length);
            Console.WriteLine(storyDetail.Categories[0].Name);
            Console.WriteLine(storyDetail.Categories[0].Id);
            Console.WriteLine(storyDetail.Description);
        }
    }

    static void TestSet2(TangThuVienCrawler crawler)
    {
        var GetStoriesOfCategoryPass = true;
        var GetStoriesBySearchNamePass = true;
        var GetStoriesOfAuthorPass = true;
        var GetChaptersOfStoryPass = true;
        GetStoriesOfCategoryPass = false;
        GetStoriesBySearchNamePass = false;
        GetStoriesOfAuthorPass = false;
        GetChaptersOfStoryPass = false;

        if (GetStoriesOfCategoryPass == false)
        {
            var storiesCategory = crawler.GetStoriesOfCategory("?ctg=1", 2, 5);
            foreach (var story in storiesCategory)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Id);
            }
        }

        if (GetStoriesBySearchNamePass == false)
        {
            var storiesSearch = crawler.GetStoriesBySearchName("Đỉnh", 2, 5);
            foreach (var story in storiesSearch)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Id);
            }
        }

        if (GetStoriesOfAuthorPass == false)
        {
            var storiesAuthor = crawler.GetStoriesOfAuthor("?author=27", 2, 5);
            foreach (var story in storiesAuthor)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Id);
            }
        }

        if (GetChaptersOfStoryPass == false)
        {
            var chaptersOfStory = crawler.GetChaptersOfStory("/dichdinh-cao-quyen-luc-suu-tam", 2, 5);
            foreach (var chapter in chaptersOfStory)
            {
                Console.WriteLine(chapter.Name);
                Console.WriteLine(chapter.Id);
            }
        }
    }
}
