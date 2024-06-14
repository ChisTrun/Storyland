using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web;
using backend.Domain.Contract;
using backend.Domain.Entities;
using backend.Domain.Objects;
using HtmlAgilityPack.CssSelectors.NetCore;
using backend.Domain.Utils;

namespace TrumTruyenHtmlCrawler;

public class TrumTruyenCrawler : ICrawler
{
    public string Name => "Trùm Truyện";

    public string Address => HOME_URL;

    // const
    const string HOME_URL = "https://trumtruyen.vn/";
    const string GET_CHAPTER_URL = "https://trumtruyen.vn/ajax.php?type=list_chapter&tid={0}&tascii={1}&tname={2}&page={3}&totalp={4}";
    const string REGEX_01 = @"https://trumtruyen\.vn/the-loai/([^/]+)";
    const string REGEX_02 = @"/trang-(\d+)";
    const string REGEX_03 = @"chuong-(\d+)";
    const string REGEX_04 = @"page=(\d+)";
    const string REGEX_05 = @"https://trumtruyen\.vn/([^/]+)";
    const string REGEX_06 = @"trang-(\d+)";
    const string REGEX_07 = @"https://trumtruyen\.vn/tac-gia/([^/]+)";
    const string REGEX_08 = @"https://trumtruyen\.vn/(.*)";

    const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
    const int DEFAULT_PAGE_SIZE = 50;
    const int DEFAULT_SEARCH_SIZE = 28;

    // Private method
    private Regex _regex01 = new Regex(REGEX_01);
    private Regex _regex02 = new Regex(REGEX_02);
    private Regex _regex03 = new Regex(REGEX_03);
    private Regex _regex04 = new Regex(REGEX_04);
    private Regex _regex05 = new Regex(REGEX_05);
    private Regex _regex06 = new Regex(REGEX_06);
    private Regex _regex07 = new Regex(REGEX_07);
    private Regex _regex08 = new Regex(REGEX_08);
    private HtmlDocument LoadHtmlDocument(string url)
    {
        var web = new HtmlWeb { UserAgent = USER_AGENT };
        return web.Load(url);
    }

    public Story GetExactStory(string id)
    {
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/{id}");
        string name = doc.QuerySelector("h3[itemprop=\"name\"]").InnerText;
        string imageUrl = doc.QuerySelector(".info-holder img").GetAttributeValue("src", "");
        string authorName = doc.QuerySelector("a[itemprop=\"author\"]").InnerText;
        return new Story(name, id, imageUrl, authorName);
    }

    public List<string> GetAuthorInfo(string storyId)
    {
        List<string> strings = new List<string>();
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/{storyId}");
        strings.Add(doc.QuerySelector("a[itemprop=\"author\"]").InnerText);
        strings.Add(_regex07.Match(doc.QuerySelector("a[itemprop=\"author\"]").GetAttributeValue("href", "")).Groups[1].Value);
        return strings;
    }



    private string FetchData(string url)
    {
        string data = "";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "C# App");
        try
        {
            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            data = response.Content.ReadAsStringAsync().Result;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request error: {ex.Message}");
            throw;
        }
        return data;
    }


    public int GetTotalChap(string id)
    {
        try {
            HtmlDocument firstPage = LoadHtmlDocument($"{HOME_URL}{id}");
            string name = firstPage.QuerySelector("h3[itemprop=\"name\"]").InnerText;
            string storyId = firstPage.QuerySelector("#truyen-id").GetAttributeValue("value", "");
            int totalPage = int.Parse(firstPage.QuerySelector("#total-page").GetAttributeValue("value", ""));
            HtmlDocument targetPage = new HtmlDocument();
            var url = string.Format(GET_CHAPTER_URL, storyId, id, WebUtility.UrlEncode(name), totalPage, totalPage);
            string data = FetchData(url);
            var jsonData = JObject.Parse(data);
            targetPage.LoadHtml(((string?)jsonData["chap_list"]));
            return (totalPage - 1) * DEFAULT_PAGE_SIZE + targetPage.QuerySelectorAll("li a").Count();
        } catch
        {
            return -1;
        }
    }

    private int GetTotalStorySearch(string name)
    {
        var searchContent = WebUtility.UrlEncode(name);
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}tim-kiem/?tukhoa={searchContent}");
        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex04.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        HtmlDocument lastPage = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}&page={lastIndex}");
        return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").Count();
    }

    private int GetTotalStoryAuthor(string name)
    {
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tac-gia/{name}");
        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        HtmlDocument lastPage = LoadHtmlDocument($"{HOME_URL}/tac-gia/{name}/trang-{lastIndex}");
        return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").Count();
    }

    private int GetTotalStoryCategory(string name)
    {
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/the-loai/{name}");
        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        HtmlDocument lastPage = LoadHtmlDocument($"{HOME_URL}/the-loai/{name}/trang-{lastIndex}");
        return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").Count();
    }


    private string RemoveVietnameseCharsAndReplaceSpaces(string input)
    {
        string normalizedString = input.Normalize(NormalizationForm.FormKD);
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        string result = stringBuilder.ToString().ToLower().Replace(" ", "-");
        return result;
    }

    // Implement

    public List<Category> GetCategories()
    {
        List<Category> categories = new List<Category>();
        var data = FetchData("https://trumtruyen.vn/ajax.php?type=select_cat_option");
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(data);
        foreach (var node in doc.QuerySelectorAll("option"))
        {
            categories.Add(new Category(node.InnerText, RemoveVietnameseCharsAndReplaceSpaces(node.InnerText)));
        }
        return categories;
    }

    public ChapterContent GetChapterContent(string storyId, int chapterIndex)
    {
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/{storyId}/chuong-{chapterIndex + 1}");
        string name = doc.QuerySelector(".chapter-title").InnerText;
        string content = doc.QuerySelector("#chapter-c").InnerHtml.Replace("<br>", "\n");
        string nextChapterUrl = doc.QuerySelector(".btn-group").QuerySelectorAll("a")[1].GetAttributeValue("href", "");
        string prevChapterUrl = doc.QuerySelector(".btn-group").QuerySelectorAll("a")[0].GetAttributeValue("href", "");
        return new ChapterContent(content, name, chapterIndex, storyId);
    }

    public List<Chapter> GetChaptersOfStory(string storyId)
    {
        Story story = GetExactStory(storyId);
        List<Chapter> chapters = new List<Chapter>();
        HtmlDocument firstPage = LoadHtmlDocument($"{HOME_URL}/{storyId}");
        string name = firstPage.QuerySelector("h3[itemprop=\"name\"]").InnerText;
        string storyCode = firstPage.QuerySelector("#truyen-id").GetAttributeValue("value", "");
        int totalPage = int.Parse(firstPage.QuerySelector("#total-page").GetAttributeValue("value", ""));
        if (totalPage == 1)
        {
            foreach (var node in firstPage.QuerySelectorAll(".list-chapter li a"))
            {
                chapters.Add(new Chapter(node.InnerText, storyId, int.Parse(_regex03.Match(node.GetAttributeValue("href", "")).Groups[1].Value) - 1));
            }
            return chapters;
        }
        for (int i = 1; i <= totalPage; i++)
        {
            HtmlDocument targetPage = new HtmlDocument();
            var url = string.Format(GET_CHAPTER_URL, storyCode, storyId, WebUtility.UrlEncode(name), i, totalPage);
            var data = FetchData(url) as string;
            var jsonData = JObject.Parse(data);
            targetPage.LoadHtml(((string?)jsonData["chap_list"]));
            foreach (var node in targetPage.QuerySelectorAll("li a"))
            {
                chapters.Add(new Chapter(node.InnerText, storyId, int.Parse(_regex03.Match(node.GetAttributeValue("href", "")).Groups[1].Value) - 1));
            }
        }
        return chapters;
    }

    public PagedList<Chapter> GetChaptersOfStory(string storyId, int page, int limit)
    {
        Story story = GetExactStory(storyId);
        int skippedElements = limit * (page - 1);
        int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_PAGE_SIZE);
        int startIndex = skippedElements % DEFAULT_PAGE_SIZE;
        int totalChap = GetTotalChap(storyId);
        int totalPage = (int)Math.Ceiling((double)totalChap / limit);
        List<Chapter> chapters = new List<Chapter>();
        if (skippedElements > totalChap)
            return new PagedList<Chapter>(page, limit, totalPage, chapters);
        int count = 0;
        HtmlDocument firstPage = LoadHtmlDocument($"{HOME_URL}/{storyId}");
        string name = firstPage.QuerySelector("h3[itemprop=\"name\"]").InnerText;
        string storyCode = firstPage.QuerySelector("#truyen-id").GetAttributeValue("value", "");
        while (count < limit)
        {
            HtmlDocument targetPage = new HtmlDocument();
            var url = string.Format(GET_CHAPTER_URL, storyCode, storyId, WebUtility.UrlEncode(name), startPage + 1, totalPage);
            var data = FetchData(url) as string;
            var jsonData = JObject.Parse(data);
            targetPage.LoadHtml(((string?)jsonData["chap_list"]));
            var nodes = targetPage.QuerySelectorAll(".list-chapter li a");
            if (startIndex >= nodes.Count)
                break;
            for (; startIndex < DEFAULT_PAGE_SIZE; startIndex++)
            {
                count++;
                if (count > limit || startIndex >= nodes.Count)
                    break;
                chapters.Add(new Chapter(nodes[startIndex].InnerText, storyId, int.Parse(_regex03.Match(nodes[startIndex].GetAttributeValue("href", "")).Groups[1].Value) - 1));
            }
            startIndex = 0;
            startPage += 1;
        }
        return new PagedList<Chapter>(page, limit, totalPage, chapters);
    }

    public List<Story> GetStoriesBySearchName(string storyName)
    {
        List<Task<List<Story>>> tasks = new List<Task<List<Story>>>();
        List<Story> result = new List<Story>();
        var searchContent = WebUtility.UrlEncode(storyName);
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}");
        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex04.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        for (int i = 1; i <= lastIndex; i++)
        {
            var iCopy = i;
            tasks.Add(Task.Run(() =>
            {
                List<Story> stories = new List<Story>();
                HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}&page={iCopy}");
                foreach (var node in curPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]"))
                {
                    string imageLink = "";
                    if (node.QuerySelectorAll("img").Count() != 0)
                    {
                        imageLink = node.QuerySelector("img").GetAttributeValue("src", "");
                    }
                    else
                    {
                        imageLink = node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
                    }
                    var name = $"{node.QuerySelector("h3").InnerText}";
                    var authorName = node.QuerySelector("span[class=\"author\"]").InnerText;
                    var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                    var numberOfChapter = GetTotalChap(id);
                    stories.Add(new Story(name, id, imageLink, authorName,numberOfChapter));
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

    public PagedList<Story> GetStoriesBySearchName(string storyName, int page, int limit)
    {
        var searchContent = WebUtility.UrlEncode(storyName);
        int skippedElements = limit * (page - 1);
        int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
        int startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
        int totalStory = GetTotalStorySearch(storyName);
        int totalPage = (int)Math.Ceiling((double)totalStory / limit);
        List<Story> stories = new List<Story>();
        if (skippedElements > totalStory)
            return new PagedList<Story>(page, limit, totalPage, stories);
        int count = 0;
        while (count < limit)
        {
            if (startPage >= totalPage) break;
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}tim-kiem/?tukhoa={searchContent}&page={startPage + 1}");
            var nodes = doc.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").ToList();
            for (; startIndex < nodes.Count; startIndex++)
            {
                count++;
                if (count > limit || count > totalStory || startIndex >= nodes.Count())
                    break;
                HtmlNode node = nodes[startIndex];
                string imageLink = "";
                if (node.QuerySelectorAll("img").Count() != 0)
                {
                    imageLink = node.QuerySelector("img").GetAttributeValue("src", "");
                }
                else
                {
                    imageLink = node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
                }
                var name = $"{node.QuerySelector("h3").InnerText}";
                var authorName = node.QuerySelector("span[class=\"author\"]").InnerText;
                var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                stories.Add(new Story(name, id, imageLink, authorName));
            }
            startIndex = 0;
            startPage += 1;
        }
        return new PagedList<Story>(page, limit, totalPage, stories);
    }

    public List<Story> GetStoriesOfAuthor(string authorId)
    {
        List<Task<List<Story>>> tasks = new List<Task<List<Story>>>();
        List<Story> result = new List<Story>();
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tac-gia/{authorId}");
        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        for (int i = 1; i <= lastIndex; i++)
        {
            var iCopy = i;
            tasks.Add(Task.Run(() =>
            {
                List<Story> stories = new List<Story>();
                HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/tac-gia/{authorId}/trang-{iCopy}");
                foreach (var node in curPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]"))
                {
                    string imageLink = "";
                    if (node.QuerySelectorAll("img").Count() != 0)
                    {
                        imageLink = node.QuerySelector("img").GetAttributeValue("src", "");
                    }
                    else
                    {
                        imageLink = node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
                    }
                    var name = $"{node.QuerySelector("h3").InnerText}";
                    var authorName = node.QuerySelector("span[class=\"author\"]").InnerText;
                    var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                    stories.Add(new Story(name, id, imageLink, authorName));
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

    public PagedList<Story> GetStoriesOfAuthor(string authorId, int page, int limit)
    {
        int skippedElements = limit * (page - 1);
        int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
        int startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
        int totalStory = GetTotalStoryAuthor(authorId);
        int totalPage = (int)Math.Ceiling((double)totalStory / limit);
        List<Story> stories = new List<Story>();
        if (skippedElements > totalStory)
            return new PagedList<Story>(page, limit, totalPage, stories);
        int count = 0;
        while (count < limit)
        {
            if (startPage >= totalPage) break;
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tac-gia/{authorId}/trang-{startPage + 1}");
            ;
            var nodes = doc.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").ToList();
            if (startIndex >= nodes.Count())
                break;
            for (; startIndex < nodes.Count; startIndex++)
            {
                count++;
                if (count > limit || count > totalStory || startIndex >= nodes.Count())
                    break;
                HtmlNode node = nodes[startIndex];
                string imageLink = "";
                if (node.QuerySelectorAll("img").Count() != 0)
                {
                    imageLink = node.QuerySelector("img").GetAttributeValue("src", "");
                }
                else
                {
                    imageLink = node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
                }
                var name = $"{node.QuerySelector("h3").InnerText}";
                var authorName = node.QuerySelector("span[class=\"author\"]").InnerText;
                var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                stories.Add(new Story(name, id, imageLink, authorName));
            }
            startIndex = 0;
            startPage += 1;
        }
        return new PagedList<Story>(page, limit, totalPage, stories);
    }

    public List<Story> GetStoriesOfCategory(string categoryId)
    {
        List<Task<List<Story>>> tasks = new List<Task<List<Story>>>();
        List<Story> result = new List<Story>();
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/the-loai/{categoryId}");
        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        for (int i = 1; i <= lastIndex; i++)
        {
            var iCopy = i;
            tasks.Add(Task.Run(() =>
            {
                List<Story> stories = new List<Story>();
                HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/the-loai/{categoryId}/trang-{iCopy}");
                foreach (var node in curPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]"))
                {
                    string imageLink = "";
                    if (node.QuerySelectorAll("img").Count() != 0)
                    {
                        imageLink = node.QuerySelector("img").GetAttributeValue("src", "");
                    }
                    else
                    {
                        imageLink = node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
                    }
                    var name = $"{node.QuerySelector("h3").InnerText}";
                    var authorName = node.QuerySelector("span[class=\"author\"]").InnerText;
                    var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                    stories.Add(new Story(name, id, imageLink, authorName));
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

    public PagedList<Story> GetStoriesOfCategory(string categoryId, int page, int limit)
    {
        int skippedElements = limit * (page - 1);
        int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
        int startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
        int totalStory = GetTotalStoryCategory(categoryId);
        int totalPage = (int)Math.Ceiling((double)totalStory / limit);
        List<Story> stories = new List<Story>();
        if (skippedElements > totalStory)
            return new PagedList<Story>(page, limit, totalPage, stories);
        int count = 0;
        while (count < limit)
        {
            if (startPage >= totalPage) break;
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/the-loai/{categoryId}/trang-{startPage + 1}");
            ;
            var nodes = doc.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").ToList();
            if (startIndex >= nodes.Count())
                break;
            for (; startIndex < nodes.Count; startIndex++)
            {
                count++;
                if (count > limit || count > totalStory || startIndex >= nodes.Count())
                    break;
                HtmlNode node = nodes[startIndex];
                string imageLink = "";
                if (node.QuerySelectorAll("img").Count() != 0)
                {
                    imageLink = node.QuerySelector("img").GetAttributeValue("src", "");
                }
                else
                {
                    imageLink = node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
                }
                var name = $"{node.QuerySelector("h3").InnerText}";
                var authorName = node.QuerySelector("span[class=\"author\"]").InnerText;
                var id = $"{_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                stories.Add(new Story(name, id, imageLink, authorName));
            }
            startIndex = 0;
            startPage += 1;
        }
        return new PagedList<Story>(page, limit, totalPage, stories);
    }

    public StoryDetail GetStoryDetail(string storyId)
    {
        var story = GetExactStory(storyId);
        var doc = LoadHtmlDocument($"{HOME_URL}{storyId}");
        var dataNode = doc.QuerySelector(".text-base");
        var authorNode = doc.QuerySelector("a[itemprop=\"author\"]");
        Author author = new Author(authorNode.InnerText, _regex07.Match(authorNode.GetAttributeValue("href", "")).Groups[1].Value);
        var categoriesNode = doc.QuerySelectorAll(".info > div")[1].QuerySelectorAll("a");
        List<Category> categories = new List<Category>();
        foreach (var category in categoriesNode)
        {
            categories.Add(new Category(category.InnerText, _regex01.Match(category.GetAttributeValue("href", "")).Groups[1].Value));
        }
        string status = doc.QuerySelectorAll(".info > div")[2].QuerySelector("span").InnerText;
        string description = doc.QuerySelector("div[itemprop=\"description\"]").InnerHtml.Replace("<br>", "\n");
        return new StoryDetail(story, author, status, categories.ToArray(), description);
    }

    //public ChapterContent GetChapterContent(string chapterId)
    //{
    //    HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/{chapterId}");
    //    string name = doc.QuerySelector(".chapter-title").InnerText;
    //    Console.WriteLine(name);
    //    string content = doc.QuerySelector("#chapter-c").InnerHtml.Replace("<br>", "\n");
    //    string nextChapterUrl = doc.QuerySelector(".btn-group").QuerySelectorAll("a")[1].GetAttributeValue("href", "");
    //    string prevChapterUrl = doc.QuerySelector(".btn-group").QuerySelectorAll("a")[0].GetAttributeValue("href", "");
    //    return new ChapterContent(content,
    //        _regex08.Match(nextChapterUrl).Groups[1].Value,
    //        _regex08.Match(prevChapterUrl).Groups[1].Value, name,
    //        chapterId
    //   );
    //}

    public List<Author> GetAuthorsBySearchName(string authorName)
    {
        List<Task<List<Author>>> tasks = new List<Task<List<Author>>>();
        List<Author> result = new List<Author>();
        var searchContent = WebUtility.UrlDecode(authorName);
        HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}");
        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex04.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        for (int i = 1; i <= lastIndex; i++)
        {
            var iCopy = i;
            tasks.Add(Task.Run(() =>
            {
                List<Author> authors = new List<Author>();
                HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}&page={iCopy}");
                foreach (var node in curPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]"))
                {
                    var authorInfo = GetAuthorInfo(_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value);
                    authors.Add(new Author(authorInfo[0], authorInfo[1]));
                }
                return authors;
            }));
        }
        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            result.AddRange(task.Result);
        }
        return result;
    }

    public PagedList<Author> GetAuthorsBySearchName(string authorName, int page, int limit)
    {
        var searchContent = WebUtility.UrlDecode(authorName);
        int skippedElements = limit * (page - 1);
        int startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
        int startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
        int totalStory = GetTotalStorySearch(searchContent);
        int totalPage = (int)Math.Ceiling((double)totalStory / limit);
        List<Author> authors = new List<Author>();
        if (skippedElements > totalStory)
            return new PagedList<Author>(page, limit, totalPage, authors);
        int count = 0;
        while (count < limit)
        {
            if (startPage >= totalPage) break;
            HtmlDocument doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}&page={startPage + 1}");
            var nodes = doc.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").ToList();
            if (startIndex >= nodes.Count())
                break;
            for (; startIndex < nodes.Count; startIndex++)
            {
                count++;
                if (count > limit || count > totalStory || startIndex >= nodes.Count())
                    break;
                HtmlNode node = nodes[startIndex];
                var authorInfo = GetAuthorInfo(_regex05.Match(node.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value);
                authors.Add(new Author(authorInfo[0], authorInfo[1]));
            }
            startIndex = 0;
            startPage += 1;
        }
        return new PagedList<Author>(page, limit, totalPage, authors);
    }

    // todo: Implement this

    public int GetChaptersCount(string storyId)
    {
        return GetTotalChap(storyId);
    }

    public PagedList<Story> GetStoriesBySearchNameWithFilter(string storyName, int minChapNum, int maxChapNum, int page, int limit)
    {
        var stories = GetStoriesBySearchName(storyName);
        var filterStories = stories.Where(story => story.NumberOfChapter >= minChapNum && story.NumberOfChapter <= maxChapNum).ToList();
        int totalStory = filterStories.Count;
        int totalPage = (int)Math.Ceiling((double)totalStory / limit);
        filterStories = filterStories.Skip((page - 1) * limit).Take(limit).ToList();
        return new PagedList<Story>(page, limit, totalPage, filterStories);
    }
}
