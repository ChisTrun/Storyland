using HeadlessBrowser;
using PluginBase.Models;
using PuppeteerSharp;
using System.Text;
using System.Xml.Xsl;
using TangThuVienPlugin;

namespace PuppeteerSharpProject
{
    class Program
    {
        static void Main(string[] args)
        {
            MainTest();
        }

        static void MainTest()
        {
            Console.OutputEncoding = Encoding.Unicode;

            var t = new TangThuVienCrawler();

        }

        static void Tested()
        {
            Console.OutputEncoding = Encoding.Unicode;
            var t = new TangThuVienCrawler();

            var categories = t.GetCategories();
            foreach (var category in categories)
            {
                Console.WriteLine(category.Url);
                Console.WriteLine(category.Name);
            }

            var content1 = t.GetChapterContent("https://truyen.tangthuvien.vn/doc-truyen/ta-von-khong-y-thanh-tien/chuong-1");
            var content2 = t.GetChapterContent("https://truyen.tangthuvien.vn/doc-truyen/ta-von-khong-y-thanh-tien/chuong-0");
            Console.WriteLine(content1.PreChapUrl);
            Console.WriteLine(content1.NextChapUrl);
            Console.WriteLine(content2.PreChapUrl);
            Console.WriteLine(content2.NextChapUrl);

            var chapterInfos = t.GetChaptersOfStory("https://truyen.tangthuvien.vn/doc-truyen/gia-toc-tu-tien-tong-thi-truong-thanh");
            foreach (var chapterInfo in chapterInfos)
            {
                Console.WriteLine(chapterInfo.Name);
                Console.WriteLine(chapterInfo.Url);
            }

            IEnumerable<Story> storyInfos = t.GetStoriesBySearchName("Đỉnh");
            foreach (var storyInfo in storyInfos)
            {
                Console.WriteLine(storyInfo.Name);
                Console.WriteLine(storyInfo.Url);
            }

            IEnumerable<Author> authorInfos = t.GetAuthorsBySearchName("Đỉnh");
            foreach (var authorInfo in authorInfos)
            {
                Console.WriteLine(authorInfo.Name);
                Console.WriteLine(authorInfo.Url);
            }

            var storyInfos_Author = t.GetStoriesOfAuthor("https://truyen.tangthuvien.vn/tac-gia?author=65");
            foreach (var storyInfo in storyInfos_Author)
            {
                Console.WriteLine(storyInfo.Name);
                Console.WriteLine(storyInfo.Url);
            }

            var storiesCategories = t.GetStoriesOfCategory("https://truyen.tangthuvien.vn/tong-hop?ctg=1");
            foreach (var story in storiesCategories)
            {
                Console.WriteLine(story.Name);
            }

            var storyCategoriesPage = t.GetStoryInfoOfCategoryByPage("https://truyen.tangthuvien.vn/tong-hop?ctg=1", 1699, 2);
            foreach (var story in storyCategoriesPage)
            {
                Console.WriteLine(story.Name);
                Console.WriteLine(story.Url);
            }
        }
    }
}
