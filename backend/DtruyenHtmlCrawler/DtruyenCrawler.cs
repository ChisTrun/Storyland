using PluginBase.Contract;
using PluginBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Reflection.Metadata;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Text.Encodings.Web;
using System.Net;
using static System.Formats.Asn1.AsnWriter;
using System.Security.Cryptography.X509Certificates;

namespace DtruyenHtmlCrawler
{

    public class DtruyenCrawler : ICrawler
    {
        // const
        const string HOME_URL = "https://truyencv.vn/";
        const string REGEX_01 = @"https://truyencv\.vn/the-loai/([^/]+)";
        const string REGEX_02 = @"\?page=(\d+)";
        const string REGEX_03 = @"chuong-(\d+)";
        const string REGEX_04 = @";page=(\d+)";
        const string REGEX_05 = @"https://truyencv\.vn/([^/]+)";
        const string REGEX_06 = @"trang-(\d+)";
        const string REGEX_07 = @"https://truyencv\.vn/tac-gia/([^/]+)";

        const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
        const int DEFAULT_PAGE_SIZE = 50;
        const int DEFAULT_SEARCH_SIZE = 20;

        // Private method
        private Regex _regex01 = new Regex(REGEX_01);
        private Regex _regex02 = new Regex(REGEX_02);
        private Regex _regex03 = new Regex(REGEX_03);
        private Regex _regex04 = new Regex(REGEX_04);
        private Regex _regex05 = new Regex(REGEX_05);
        private Regex _regex06 = new Regex(REGEX_06);   
        private Regex _regex07 = new Regex(REGEX_07);
        private HtmlDocument LoadHtmlDocument(string url)
        {
            var web = new HtmlWeb { UserAgent = USER_AGENT };
            return web.Load(url);
        }
        private Story GetExactStory(string id)
        {
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/{id}");
            string name = doc.QuerySelector("h3[itemprop=\"name\"]").InnerText;
            string imageUrl = doc.QuerySelector("meta[property=\"og:image\"]").GetAttributeValue("content", "");
            return new Story(name, id, imageUrl);
        }

        private int GetTotalChap(string id)
        {
            HtmlDocument firstPage = LoadHtmlDocument($"{HOME_URL}/{id}");
            var pagingBtn = firstPage.QuerySelectorAll("#danh-sach-chuong > ul > li");
            if (pagingBtn.Count > 0)
            {
                var LinkList = firstPage.QuerySelectorAll("#danh-sach-chuong > ul > li > a");
                string link = LinkList[LinkList.Count - 1].GetAttributeValue("href", "");
                int pageIndex = int.Parse(_regex02.Match(link).Groups[1].Value);
                HtmlDocument lastPage = LoadHtmlDocument($"{HOME_URL}{LinkList[LinkList.Count - 1].GetAttributeValue("href", "")}");
                return (pageIndex - 1) * DEFAULT_PAGE_SIZE + lastPage.QuerySelectorAll("#danh-sach-chuong > div")[1].QuerySelectorAll("ul li a").Count();
            }
            return firstPage.QuerySelectorAll("#danh-sach-chuong > div")[1].QuerySelectorAll("ul li a").Count();
        }

        private int GetTotalStorySearch(string name)
        {
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem?tukhoa={name}");
            var pagingBtn = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > ul > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex04.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            HtmlDocument lastPage = LoadHtmlDocument($"{HOME_URL}/tim-kiem?tukhoa={name}&page={lastIndex}");
            return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1).Count();
        }

        private int GetTotalStoryAuthor(string name)
        {
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}tac-gia/{name}");
            var pagingBtn = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > ul > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            HtmlDocument lastPage = LoadHtmlDocument($"{HOME_URL}tac-gia/{name}/trang-{lastIndex}");
            return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1).Count();
        }

        private int GetTotalStoryCategory(string name)
        {
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}the-loai/{name}");
            var pagingBtn = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > ul > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            HtmlDocument lastPage = LoadHtmlDocument($"{HOME_URL}the-loai/{name}/trang-{lastIndex}");
            return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1).Count();
        }

        private string ExtractChapterContent(HtmlDocument doc)
        {
            StringBuilder contentBuilder = new StringBuilder();
            foreach (var node in doc.QuerySelectorAll("#content-chapter p"))
            {
                contentBuilder.AppendLine(node.InnerHtml);
            }
            return contentBuilder.ToString();
        }

        // Implement

        public string Name => "Truyenchuhay.vn";

        public string Description => "This plugin crawl data from truyenchuhay.vn";

        public IEnumerable<Category> GetCategories()
        {
            HtmlDocument doc = LoadHtmlDocument(HOME_URL);
            return doc.QuerySelectorAll("#nav-category-children-not-pc li a")
                      .Select(node => new Category(
                          node.InnerHtml,
                          _regex01.Match(node.GetAttributeValue("href", "")).Groups[1].Value
                      ))
                      .ToList();
        }

        public ChapterContent GetChapterContent(string storyId, int chapterIndex)
        {
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/{storyId}/chuong-{chapterIndex}");
            string content = ExtractChapterContent(doc);
            string nextChapterUrl = doc.QuerySelector(".flex.justify-center.h-12").QuerySelectorAll("a")[1].GetAttributeValue("href", "");
            string prevChapterUrl = doc.QuerySelector(".flex.justify-center.h-12").QuerySelectorAll("a")[0].GetAttributeValue("href", "");
            return new ChapterContent(content, nextChapterUrl, prevChapterUrl);
        }

        public IEnumerable<Chapter> GetChaptersOfStory(string storyId)
        {
            Story story = GetExactStory(storyId);
            List<Task<List<Chapter>>> tasks = new List<Task<List<Chapter>>>();
            List<Chapter> result = new List<Chapter>();
            HtmlDocument firstPage = LoadHtmlDocument($"{HOME_URL}/{storyId}");
            var pagingBtn = firstPage.QuerySelectorAll("#danh-sach-chuong > ul > li");
            var LinkList = firstPage.QuerySelectorAll("#danh-sach-chuong > ul > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex02.Match(LinkList[LinkList.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            for (int i = 1; i <= lastIndex; i++)
            {
                var iCopy = i;
                tasks.Add(Task.Run(() =>
                {
                    List<Chapter> chapters = new List<Chapter>();
                    HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/{storyId}?page={iCopy}/#danh-sach-chuong");
                    foreach (var node in curPage.QuerySelectorAll("#danh-sach-chuong > div")[1].QuerySelectorAll("ul li a"))
                    {
                        chapters.Add(new Chapter(node.InnerText, node.GetAttributeValue("href", ""), story, int.Parse(_regex03.Match(node.GetAttributeValue("href", "")).Groups[1].Value) - 1));
                    }
                    return chapters;
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                result.AddRange(task.Result);
            }
            return result;
        }

        public PagingRepresentative<Chapter> GetChaptersOfStory(string storyId, int page, int limit)
        {
            Story story = GetExactStory(storyId);
            int skippedElements = limit * (page - 1);
            int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_PAGE_SIZE);
            int startIndex = skippedElements % DEFAULT_PAGE_SIZE;
            int totalChap = GetTotalChap(storyId);
            int totalPage = (int)Math.Ceiling((double)totalChap / limit);
            List<Chapter> chapters = new List<Chapter>();
            if (skippedElements > totalChap) return new PagingRepresentative<Chapter>(page, limit, totalPage, chapters);
            int chapterIndex = page * limit + 1;
            int count = 0;
            while (count < limit)
            {
                HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/{storyId}?page={startPage + 1}/#danh-sach-chuong");
                var nodes = doc.QuerySelectorAll("#danh-sach-chuong > div")[1].QuerySelectorAll("ul li a");
                if (startIndex >= nodes.Count) break;
                for (; startIndex < DEFAULT_PAGE_SIZE; startIndex++)
                {
                    count++;
                    if (count > limit || startIndex >= nodes.Count) break;
                    HtmlNode node = doc.QuerySelectorAll("#danh-sach-chuong > div")[1].QuerySelectorAll("ul li a")[startIndex];
                    chapters.Add(new Chapter(node.InnerText, node.GetAttributeValue("href", ""), story, count + skippedElements - 1));
                }
                startIndex = 0;
                startPage += 1;
                chapterIndex++;
            }
            return new PagingRepresentative<Chapter>(page, limit, totalPage, chapters);
        }

        public IEnumerable<Story> GetStoriesBySearchName(string storyName)
        {
            List<Task<List<Story>>> tasks = new List<Task<List<Story>>>();
            List<Story> result = new List<Story>(); 
            var searchContent = WebUtility.UrlDecode(storyName);
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem?tukhoa={searchContent}");
            var pagingBtn = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > ul > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex04.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            for (int i = 1; i <= lastIndex; i++) {

                var iCopy = i;
                tasks.Add(Task.Run(() =>
                {
                    List<Story> stories = new List<Story>();
                    HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/tim-kiem?tukhoa={searchContent}&page={iCopy}");
                    foreach (var node in curPage.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1))
                    {
                        var imageLink = $"{node.QuerySelector("img").GetAttributeValue("src", "")}";
                        var name = $"{node.QuerySelector("h3").InnerText}";
                        var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                        stories.Add(new Story(name, id, imageLink));
                    }
                    return stories;
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                result.AddRange(task.Result);
            }
            return result;
        }

        public PagingRepresentative<Story> GetStoriesBySearchName(string storyName, int page, int limit)
        {
            var searchContent = WebUtility.UrlDecode(storyName);
            int skippedElements = limit * (page - 1);
            int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
            int startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
            int totalStory = GetTotalStorySearch(searchContent);
            int totalPage = (int)Math.Ceiling((double)totalStory / limit);
            List<Story> stories = new List<Story>();
            if (skippedElements > totalStory) return new PagingRepresentative<Story>(page, limit, totalPage, stories);
            int count = 0;
            while (count < limit)
            {
                HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem?tukhoa={searchContent}&page={startPage + 1}");
                var nodes = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1).ToList();
                if (startIndex >= nodes.Count()) break;
                for (; startIndex < DEFAULT_SEARCH_SIZE; startIndex++)
                {
                    count++;
                    if (count > limit || startIndex >= nodes.Count()) break;
                    HtmlNode node = nodes[startIndex];
                    var imageLink = $"{node.QuerySelector("img").GetAttributeValue("src", "")}";
                    var name = $"{node.QuerySelector("h3").InnerText}";
                    var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                    stories.Add(new Story(name, id, imageLink));
                }
                startIndex = 0;
                startPage += 1;
            }
            return new PagingRepresentative<Story>(page, limit, totalPage, stories);
        }

        public IEnumerable<Story> GetStoriesOfAuthor(string authorId)
        {
            List<Task<List<Story>>> tasks = new List<Task<List<Story>>>();
            List<Story> result = new List<Story>();
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tac-gia/{authorId}");
            var pagingBtn = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > ul > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            for (int i = 1; i <= lastIndex; i++)
            {
                var iCopy = i;
                tasks.Add(Task.Run(() =>
                {
                    List<Story> stories = new List<Story>();
                    HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/tac-gia/{authorId}/trang-{iCopy}");
                    foreach (var node in curPage.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1))
                    {
                        var imageLink = $"{node.QuerySelector("img").GetAttributeValue("src", "")}";
                        var name = $"{node.QuerySelector("h3").InnerText}";
                        var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                        stories.Add(new Story(name, id, imageLink));
                    }
                    return stories;
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                result.AddRange(task.Result);
            }
            return result;
        }

        public PagingRepresentative<Story> GetStoriesOfAuthor(string authorId, int page, int limit)
        {
            int skippedElements = limit * (page - 1);
            int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
            int startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
            int totalStory = GetTotalStoryAuthor(authorId);
            int totalPage = (int)Math.Ceiling((double)totalStory / limit);
            List<Story> stories = new List<Story>();
            if (skippedElements > totalStory) return new PagingRepresentative<Story>(page, limit, totalPage, stories);
            int count = 0;
            while (count < limit)
            {
                HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}tac-gia/{authorId}/trang-{startPage + 1}");
                var nodes = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1).ToList();
                if (startIndex >= nodes.Count()) break;
                for (; startIndex < DEFAULT_SEARCH_SIZE; startIndex++)
                {
                    count++;
                    if (count > limit || startIndex >= nodes.Count()) break;
                    HtmlNode node = nodes[startIndex];
                    var imageLink = $"{node.QuerySelector("img").GetAttributeValue("src", "")}";
                    var name = $"{node.QuerySelector("h3").InnerText}";
                    var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                    stories.Add(new Story(name, id, imageLink));
                }
                startIndex = 0;
                startPage += 1;
            }
            return new PagingRepresentative<Story>(page, limit, totalPage, stories);
        }

        public IEnumerable<Story> GetStoriesOfCategory(string categoryId)
        {
            List<Task<List<Story>>> tasks = new List<Task<List<Story>>>();
            List<Story> result = new List<Story>();
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/the-loai/{categoryId}");
            var pagingBtn = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > ul > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            for (int i = 1; i <= lastIndex; i++)
            {
                var iCopy = i;
                tasks.Add(Task.Run(() =>
                {
                    List<Story> stories = new List<Story>();
                    HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/the-loai/{categoryId}/trang-{iCopy}");
                    foreach (var node in curPage.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1))
                    {
                        var imageLink = $"{node.QuerySelector("img").GetAttributeValue("src", "")}";
                        var name = $"{node.QuerySelector("h3").InnerText}";
                        var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                        stories.Add(new Story(name, id, imageLink));
                    }
                    return stories;
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                result.AddRange(task.Result);
            }
            return result;
        }

        public PagingRepresentative<Story> GetStoriesOfCategory(string categoryId, int page, int limit)
        {
            int skippedElements = limit * (page - 1);
            int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
            int startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
            int totalStory = GetTotalStoryCategory(categoryId);
            int totalPage = (int)Math.Ceiling((double)totalStory / limit);
            List<Story> stories = new List<Story>();
            if (skippedElements > totalStory) return new PagingRepresentative<Story>(page, limit, totalPage, stories);
            int count = 0;
            while (count < limit)
            {
                HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}the-loai/{categoryId}/trang-{startPage + 1}");
                var nodes = doc.QuerySelectorAll("div[class=\"lg:col-span-9\"] > div ").Skip(1).ToList();
                if (startIndex >= nodes.Count()) break;
                for (; startIndex < DEFAULT_SEARCH_SIZE; startIndex++)
                {
                    count++;
                    if (count > limit || startIndex >= nodes.Count()) break;
                    HtmlNode node = nodes[startIndex];
                    var imageLink = $"{node.QuerySelector("img").GetAttributeValue("src", "")}";
                    var name = $"{node.QuerySelector("h3").InnerText}";
                    var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                    stories.Add(new Story(name, id, imageLink));
                }
                startIndex = 0;
                startPage += 1;
            }
            return new PagingRepresentative<Story>(page, limit, totalPage, stories);
        }

        public StoryDetail GetStoryDetail(string storyId)
        {
            //throw new NotImplementedException();
            //https://truyencv.vn/thon-thien-long-vuong
            var story = GetExactStory(storyId);
            var doc = LoadHtmlDocument($"{HOME_URL}{storyId}");
            var dataNode = doc.QuerySelector(".text-base");
            var authorNode = doc.QuerySelector("div[itemtype=\"https://schema.org/Person\"] > a");
            Author author = new Author(authorNode.InnerText, _regex07.Match(authorNode.GetAttributeValue("href", "")).Groups[1].Value);
            var categoriesNdoe = doc.QuerySelectorAll("div > a[itemprop=\"genre\"]");
            List<Category> categories = new List<Category>();   
            foreach (var category in categoriesNdoe)
            {
                categories.Add(new Category(category.InnerText, _regex01.Match(category.GetAttributeValue("href", "")).Groups[1].Value)); 
            }
            string status = doc.QuerySelectorAll(".text-base > div")[3].QuerySelector("span").InnerText;
            string description = doc.QuerySelector("#gioi-thieu-truyen").InnerText;
            return new StoryDetail(story, author, status, categories.ToArray(), description);
        }
    }
}
