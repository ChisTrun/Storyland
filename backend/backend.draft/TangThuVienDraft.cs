using PluginBase.Models;
using TangThuVien;

namespace backend.draft
{
    public class TangThuVienDraft
    {
        public static void TestSet1(TangThuVienCrawler crawler)
        {
            var GetCategoriesPass = true;
            var GetStoriesOfCategoryPass = true;
            var GetStoriesBySearchNamePass = true;
            var GetStoriesOfAuthorPass = true;
            var GetChaptersOfStoryPass = true;
            var GetChapterContentPass = true;
            var GetStoryDetailPass = true;
            GetCategoriesPass = false;
            GetStoriesOfCategoryPass = false;
            GetStoriesBySearchNamePass = false;
            GetStoriesOfAuthorPass = false;
            GetChaptersOfStoryPass = false;
            GetChapterContentPass = false;
            GetStoryDetailPass = false;

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
                var storiesCategory = crawler.GetStoriesOfCategory("quan-su");
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
                    Console.WriteLine(story.ImageUrl);
                    Console.WriteLine(story.AuthorName);
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
                var chapters = crawler.GetChaptersOfStory("thi-ra-ho-moi-la-nhan-vat-chinh");
                foreach (var chapter in chapters)
                {
                    Console.WriteLine(chapter.Name);
                    Console.WriteLine(chapter.Id);
                }
            }

            if (GetChapterContentPass == false)
            {
                //Console.WriteLine("Chương 1 : Vai Phụ Đúng Là Bản Thân Tôi" == "Chương 1 : Vai Phụ Đúng Là Bản Thân Tôi");
                var content1 = crawler.GetChapterContent("thi-ra-ho-moi-la-nhan-vat-chinh", 1);
                var content2 = crawler.GetChapterContent("thi-ra-ho-moi-la-nhan-vat-chinh", 2);
                var content3 = crawler.GetChapterContent("nhat-tich-dac-dao/260875-chuong-1");
                var content4 = crawler.GetChapterContent("nhat-tich-dac-dao", 0);
                Console.WriteLine(content1.Content);
                Console.WriteLine(content1.PrevChapID);
                Console.WriteLine(content1.NextChapID);
                Console.WriteLine(content2.Content);
                Console.WriteLine(content2.PrevChapID);
                Console.WriteLine(content2.NextChapID);
            }
            // Anh Hùng Liên Minh Chi Thái Điểu Chi Quang

            if (GetStoryDetailPass == false)
            {
                var storyDetail = crawler.GetStoryDetail("thi-ra-ho-moi-la-nhan-vat-chinh");
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

        public static void TestSet2(TangThuVienCrawler crawler)
        {
            var GetStoriesOfCategoryPass = true;
            var GetStoriesBySearchNamePass = true;
            var GetStoriesBySearchNameNotFound = true;
            var GetStoriesOfAuthorPass = true;
            var GetChaptersOfStoryPass = true;
            GetStoriesOfCategoryPass = false;
            GetStoriesBySearchNamePass = false;
            GetStoriesBySearchNameNotFound = false;
            GetStoriesOfAuthorPass = false;
            GetChaptersOfStoryPass = false;

            if (GetStoriesOfCategoryPass == false)
            {
                var storiesCategory = crawler.GetStoriesOfCategory("tien-hiep", 342, 5);
                foreach (var story in storiesCategory.Data)
                {
                    Console.WriteLine(story.Name);
                    Console.WriteLine(story.Id);
                }
                Console.WriteLine(storiesCategory.TotalPages);
            }

            if (GetStoriesBySearchNamePass == false)
            {
                var storiesSearch = crawler.GetStoriesBySearchName("Đỉnh", 47, 5);
                foreach (var story in storiesSearch.Data)
                {
                    Console.WriteLine(story.Name);
                    Console.WriteLine(story.Id);
                }
                Console.WriteLine(storiesSearch.TotalPages);
                var storiesSearch2 = crawler.GetStoriesBySearchName("Tình Anh Bán", 1, 5);
            }

            if (GetStoriesBySearchNameNotFound == false)
            {
                var storiesSearch = crawler.GetStoriesBySearchName("Đỉnh kkkk", 1, 5);
                foreach (var story in storiesSearch.Data)
                {
                    Console.WriteLine(story.Name);
                    Console.WriteLine(story.Id);
                }
                Console.WriteLine(storiesSearch.TotalPages);
            }

            if (GetStoriesOfAuthorPass == false)
            {
                var storiesAuthor = crawler.GetStoriesOfAuthor("?author=27", 5, 5);
                foreach (var story in storiesAuthor.Data)
                {
                    Console.WriteLine(story.Name);
                    Console.WriteLine(story.Id);
                }
                Console.WriteLine(storiesAuthor.TotalPages);
                var storiesAuthor2 = crawler.GetStoriesOfAuthor("?author=21981", 1, 5);
                var storiesAuthor3 = crawler.GetStoriesOfAuthor("?author=172312", 1, 5);
            }

            if (GetChaptersOfStoryPass == false)
            {
                var chaptersOfStory = crawler.GetChaptersOfStory("dichdinh-cao-quyen-luc-suu-tam", 144, 5);
                foreach (var chapter in chaptersOfStory.Data)
                {
                    Console.WriteLine(chapter.Name);
                    Console.WriteLine(chapter.Id);
                }
                Console.WriteLine(chaptersOfStory.TotalPages);
                var chaptersOfStory2 = crawler.GetChaptersOfStory("tinh-dinh-kitty-cao-lanh-tong-tai-due-due-due", 1, 5);
                var storyDetail = crawler.GetStoryDetail("tinh-dinh-kitty-cao-lanh-tong-tai-due-due-due");
                var chapterContent = crawler.GetChapterContent("tinh-dinh-kitty-cao-lanh-tong-tai-due-due-due", 1);
            }
        }

    }
}
