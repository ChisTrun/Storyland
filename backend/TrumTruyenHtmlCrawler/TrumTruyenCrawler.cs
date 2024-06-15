using Backend.Domain.Contract;
using Backend.Domain.Entities;
using Backend.Domain.Objects;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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
    const string REGEX_09 = @"\d+";

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
    private Regex _regex09 = new Regex(REGEX_09);
    private HtmlDocument LoadHtmlDocument(string url)
    {
        var web = new HtmlWeb { UserAgent = USER_AGENT };
        return web.Load(url);
    }

    public Story GetExactStory(string id)
    {
        var doc = LoadHtmlDocument($"{HOME_URL}/{id}");
        var name = doc.QuerySelector("h3[itemprop=\"name\"]").InnerText;
        var imageUrl = doc.QuerySelector(".info-holder img").GetAttributeValue("src", "");
        var authorName = doc.QuerySelector("a[itemprop=\"author\"]").InnerText;
        return new Story(name, id, imageUrl, authorName);
    }

    public List<string> GetAuthorInfo(string storyId)
    {
        var strings = new List<string>();
        var doc = LoadHtmlDocument($"{HOME_URL}/{storyId}");
        strings.Add(doc.QuerySelector("a[itemprop=\"author\"]").InnerText);
        strings.Add(_regex07.Match(doc.QuerySelector("a[itemprop=\"author\"]").GetAttributeValue("href", "")).Groups[1].Value);
        return strings;
    }



    private string FetchData(string url)
    {
        var data = "";
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
        try
        {
            var firstPage = LoadHtmlDocument($"{HOME_URL}{id}");
            var name = firstPage.QuerySelector("h3[itemprop=\"name\"]").InnerText;
            var storyId = firstPage.QuerySelector("#truyen-id").GetAttributeValue("value", "");
            var totalPage = int.Parse(firstPage.QuerySelector("#total-page").GetAttributeValue("value", ""));
            var targetPage = new HtmlDocument();
            var url = string.Format(GET_CHAPTER_URL, storyId, id, WebUtility.UrlEncode(name), totalPage, totalPage);
            var data = FetchData(url);
            var jsonData = JObject.Parse(data);
            targetPage.LoadHtml(((string?)jsonData["chap_list"]));
            return (totalPage - 1) * DEFAULT_PAGE_SIZE + targetPage.QuerySelectorAll("li a").Count();
        }
        catch
        {
            return -1;
        }
    }

    private int GetTotalStorySearch(string name)
    {
        var searchContent = WebUtility.UrlEncode(name);
        var doc = LoadHtmlDocument($"{HOME_URL}tim-kiem/?tukhoa={searchContent}");
        if (doc.Text.Contains("Error 404 (Not Found)!!"))
        {
            throw new System.Exception("invalid id");
        }

        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex04.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        var lastPage = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}&page={lastIndex}");
        return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").Count();
    }

    private int GetTotalStoryAuthor(string name)
    {
        try
        {
            var doc = LoadHtmlDocument($"{HOME_URL}/tac-gia/{name}");
            if (doc.Text.Contains("Error 404 (Not Found)!!"))
            {
                throw new System.Exception("invalid id");
            }

            var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            var lastPage = LoadHtmlDocument($"{HOME_URL}/tac-gia/{name}/trang-{lastIndex}");
            return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").Count();
        }
        catch
        {
            throw new System.Exception("some thing wrong");
        }
    }

    private int GetTotalStoryCategory(string name)
    {
        var doc = LoadHtmlDocument($"{HOME_URL}/the-loai/{name}");
        if (doc.Text.Contains("Error 404 (Not Found)!!"))
        {
            throw new System.Exception("invalid id");
        }

        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        var lastPage = LoadHtmlDocument($"{HOME_URL}/the-loai/{name}/trang-{lastIndex}");
        return (lastIndex - 1) * DEFAULT_SEARCH_SIZE + lastPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").Count();
    }


    private string RemoveVietnameseCharsAndReplaceSpaces(string input)
    {
        var normalizedString = input.Normalize(NormalizationForm.FormKD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        var result = stringBuilder.ToString()
           .Replace("đ", "d")
           .Replace("Đ", "D")
           .ToLower()
           .Replace(" ", "-");

        return result;
    }

    // Implement

    public List<Category> GetCategories()
    {
        var categories = new List<Category>();
        var data = FetchData("https://trumtruyen.vn/ajax.php?type=select_cat_option");
        var doc = new HtmlDocument();
        doc.LoadHtml(data);
        foreach (var node in doc.QuerySelectorAll("option"))
        {
            if (node.InnerText == "Tất cả" || node.InnerText == "Khác")
            {
                continue;
            }
            categories.Add(new Category(node.InnerText, RemoveVietnameseCharsAndReplaceSpaces(node.InnerText)));
        }
        return categories;
    }

    public ChapterContent GetChapterContent(string storyId, int chapterIndex)
    {
        if (chapterIndex < 0)
        {
            throw new System.Exception("invalid chapter index");
        }

        var doc = LoadHtmlDocument($"{HOME_URL}/{storyId}/chuong-{chapterIndex + 1}");
        try
        {
            var name = doc.QuerySelector(".chapter-title").InnerText;
            var content = doc.QuerySelector("#chapter-c").InnerHtml;
            content = content.Replace("<br>", "\n");
            content = Regex.Replace(content, @"<.*?>", string.Empty);
            var nextChapterUrl = doc.QuerySelector(".btn-group").QuerySelectorAll("a")[1].GetAttributeValue("href", "");
            var prevChapterUrl = doc.QuerySelector(".btn-group").QuerySelectorAll("a")[0].GetAttributeValue("href", "");
            return new ChapterContent(content, name, chapterIndex, storyId);
        }
        catch
        {
            throw new System.Exception("invalid story id");
        }

    }

    public List<Chapter> GetChaptersOfStory(string storyId)
    {
        try
        {
            var story = GetExactStory(storyId);
            var chapters = new List<Chapter>();
            var firstPage = LoadHtmlDocument($"{HOME_URL}/{storyId}");
            var name = firstPage.QuerySelector("h3[itemprop=\"name\"]").InnerText;
            var storyCode = firstPage.QuerySelector("#truyen-id").GetAttributeValue("value", "");
            var totalPage = int.Parse(firstPage.QuerySelector("#total-page").GetAttributeValue("value", ""));
            if (totalPage == 1)
            {
                foreach (var node in firstPage.QuerySelectorAll(".list-chapter li a"))
                {
                    chapters.Add(new Chapter(node.InnerText, storyId, int.Parse(_regex03.Match(node.GetAttributeValue("href", "")).Groups[1].Value) - 1));
                }
                return chapters;
            }
            for (var i = 1; i <= totalPage; i++)
            {
                var targetPage = new HtmlDocument();
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
        catch
        {
            throw new System.Exception("some thing wrong");
        }
    }

    public PagedList<Chapter> GetChaptersOfStory(string storyId, int page, int limit)
    {
        try
        {
            if (page < 1 || limit < 0)
            {
                throw new System.Exception("invalid paging data");
            }

            var story = GetExactStory(storyId);
            var skippedElements = limit * (page - 1);
            var startPage = (int)Math.Floor((double)skippedElements / DEFAULT_PAGE_SIZE);
            var startIndex = skippedElements % DEFAULT_PAGE_SIZE;
            var totalChap = GetTotalChap(storyId);
            var totalPage = (int)Math.Ceiling((double)totalChap / limit);
            var chapters = new List<Chapter>();
            if (skippedElements > totalChap)
            {
                return new PagedList<Chapter>(page, limit, totalPage, chapters);
            }

            var count = 0;
            var firstPage = LoadHtmlDocument($"{HOME_URL}/{storyId}");
            var name = firstPage.QuerySelector("h3[itemprop=\"name\"]").InnerText;
            var storyCode = firstPage.QuerySelector("#truyen-id").GetAttributeValue("value", "");
            while (count < limit)
            {
                var targetPage = new HtmlDocument();
                var url = string.Format(GET_CHAPTER_URL, storyCode, storyId, WebUtility.UrlEncode(name), startPage + 1, totalPage);
                var data = FetchData(url) as string;
                var jsonData = JObject.Parse(data);
                targetPage.LoadHtml(((string?)jsonData["chap_list"]));
                var nodes = targetPage.QuerySelectorAll(".list-chapter li a");
                if (startIndex >= nodes.Count)
                {
                    break;
                }

                for (; startIndex < DEFAULT_PAGE_SIZE; startIndex++)
                {
                    count++;
                    if (count > limit || startIndex >= nodes.Count)
                    {
                        break;
                    }

                    chapters.Add(new Chapter(nodes[startIndex].InnerText, storyId, int.Parse(_regex03.Match(nodes[startIndex].GetAttributeValue("href", "")).Groups[1].Value) - 1));
                }
                startIndex = 0;
                startPage += 1;
            }
            return new PagedList<Chapter>(page, limit, totalPage, chapters);
        }
        catch
        {
            throw new System.Exception("some thing wrong");
        }
    }

    public List<Story> GetStoriesBySearchName(string storyName)
    {
        if (storyName.Length == 0)
        {
            throw new System.Exception("invalid name");
        }

        try
        {
            var tasks = new List<Task<List<Story>>>();
            var result = new List<Story>();
            var searchContent = WebUtility.UrlEncode(storyName);
            var doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}");
            if (doc.Text.Contains("Error 404 (Not Found)!!"))
            {
                throw new System.Exception("invalid id");
            }

            var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex04.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            for (var i = 1; i <= lastIndex; i++)
            {
                var iCopy = i;
                tasks.Add(Task.Run(() =>
                {
                    List<Task<Story>> nodeTasks = new List<Task<Story>>();
                    HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}&page={iCopy}");
                    foreach (var node in curPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]"))
                    {
                        var nodeCopy = node;
                        nodeTasks.Add(Task.Run(() =>
                        {
                            string imageLink = nodeCopy.QuerySelectorAll("img").Count() != 0
                                ? nodeCopy.QuerySelector("img").GetAttributeValue("src", "")
                                : nodeCopy.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");

                            var name = $"{nodeCopy.QuerySelector("h3").InnerText}";
                            var authorName = nodeCopy.QuerySelector("span[class=\"author\"]").InnerText;
                            var id = $"{_regex05.Match(nodeCopy.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                            var numberOfChapter = int.Parse(_regex09.Match(nodeCopy.QuerySelectorAll(".author")[^1].InnerText).Groups[0].Value);
                            return new Story(name, id, imageLink, authorName, numberOfChapter);
                        }));
                    }
                    Task.WaitAll(nodeTasks.ToArray());
                    return nodeTasks.Select(nt => nt.Result).ToList();
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                result.AddRange(task.Result);
            }
            return result;
        }
        catch
        {
            throw new System.Exception("some thing wrong");
        }
    }

    public PagedList<Story> GetStoriesBySearchName(string storyName, int page, int limit)
    {
        if (storyName.Length == 0)
        {
            throw new System.Exception("invalid name");
        }

        if (page < 1 || limit < 0)
        {
            throw new System.Exception("invalid paging data");
        }

        try
        {
            var searchContent = WebUtility.UrlEncode(storyName);
            var skippedElements = limit * (page - 1);
            var startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
            var startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
            var totalStory = GetTotalStorySearch(storyName);
            var totalPage = (int)Math.Ceiling((double)totalStory / limit);
            var stories = new List<Story>();
            if (skippedElements > totalStory)
            {
                return new PagedList<Story>(page, limit, totalPage, stories);
            }

            var count = 0;
            while (count < limit)
            {
                if (startPage >= totalPage)
                {
                    break;
                }

                var doc = LoadHtmlDocument($"{HOME_URL}tim-kiem/?tukhoa={searchContent}&page={startPage + 1}");
                var nodes = doc.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").ToList();
                for (; startIndex < nodes.Count; startIndex++)
                {
                    count++;
                    if (count > limit || count > totalStory || startIndex >= nodes.Count())
                    {
                        break;
                    }

                    var node = nodes[startIndex];
                    var imageLink = node.QuerySelectorAll("img").Count() != 0
                        ? node.QuerySelector("img").GetAttributeValue("src", "")
                        : node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
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
        catch
        {
            throw new System.Exception("some thing wrong");
        }
    }

    public List<Story> GetStoriesOfAuthor(string authorId)
    {
        try
        {

            var tasks = new List<Task<List<Story>>>();
            var result = new List<Story>();
            var doc = LoadHtmlDocument($"{HOME_URL}/tac-gia/{authorId}");
            if (doc.Text.Contains("Error 404 (Not Found)!!"))
            {
                throw new System.Exception("invalid id");
            }

            var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            for (var i = 1; i <= lastIndex; i++)
            {
                var iCopy = i;
                tasks.Add(Task.Run(() =>
                {
                    List<Task<Story>> nodeTasks = new List<Task<Story>>();
                    HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/tac-gia/{authorId}/trang-{iCopy}");
                    foreach (var node in curPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]"))
                    {
                        var nodeCopy = node;
                        nodeTasks.Add(Task.Run(() =>
                        {
                            string imageLink = nodeCopy.QuerySelectorAll("img").Count() != 0
                                ? nodeCopy.QuerySelector("img").GetAttributeValue("src", "")
                                : nodeCopy.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");

                            var name = $"{nodeCopy.QuerySelector("h3").InnerText}";
                            var authorName = nodeCopy.QuerySelector("span[class=\"author\"]").InnerText;
                            var id = $"{_regex05.Match(nodeCopy.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                            var numberOfChapter = int.Parse(_regex09.Match(nodeCopy.QuerySelectorAll(".author")[^1].InnerText).Groups[0].Value);
                            return new Story(name, id, imageLink, authorName, numberOfChapter);
                        }));
                    }
                    Task.WaitAll(nodeTasks.ToArray());
                    return nodeTasks.Select(nt => nt.Result).ToList();
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                result.AddRange(task.Result);
            }
            return result;
        }
        catch
        {
            throw new System.Exception("some thing wrong");
        }
    }

    public PagedList<Story> GetStoriesOfAuthor(string authorId, int page, int limit)
    {
        if (page < 1 || limit < 0)
        {
            throw new System.Exception("invalid paging data");
        }

        try
        {
            var skippedElements = limit * (page - 1);
            var startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
            var startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
            var totalStory = GetTotalStoryAuthor(authorId);
            var totalPage = (int)Math.Ceiling((double)totalStory / limit);
            var stories = new List<Story>();
            if (skippedElements > totalStory)
            {
                return new PagedList<Story>(page, limit, totalPage, stories);
            }

            var count = 0;
            while (count < limit)
            {
                if (startPage >= totalPage)
                {
                    break;
                }

                var doc = LoadHtmlDocument($"{HOME_URL}/tac-gia/{authorId}/trang-{startPage + 1}");
                ;
                var nodes = doc.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").ToList();
                if (startIndex >= nodes.Count())
                {
                    break;
                }

                for (; startIndex < nodes.Count; startIndex++)
                {
                    count++;
                    if (count > limit || count > totalStory || startIndex >= nodes.Count())
                    {
                        break;
                    }

                    var node = nodes[startIndex];
                    var imageLink = node.QuerySelectorAll("img").Count() != 0
                        ? node.QuerySelector("img").GetAttributeValue("src", "")
                        : node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
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
        catch
        {
            throw new System.Exception("some thing wrong");
        }
    }

    public List<Story> GetStoriesOfCategory(string categoryId)
    {
        try
        {
            var tasks = new List<Task<List<Story>>>();
            var result = new List<Story>();
            var doc = LoadHtmlDocument($"{HOME_URL}/the-loai/{categoryId}");
            if (doc.Text.Contains("Error 404 (Not Found)!!"))
            {
                throw new System.Exception("invalid id");
            }

            var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
            var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex06.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
            for (var i = 1; i <= lastIndex; i++)
            {
                var iCopy = i;
                tasks.Add(Task.Run(() =>
                {
                    List<Task<Story>> nodeTasks = new List<Task<Story>>();
                    HtmlDocument curPage = LoadHtmlDocument($"{HOME_URL}/the-loai/{categoryId}/trang-{iCopy}");
                    foreach (var node in curPage.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]"))
                    {
                        var nodeCopy = node;
                        nodeTasks.Add(Task.Run(() =>
                        {
                            string imageLink = nodeCopy.QuerySelectorAll("img").Count() != 0
                                ? nodeCopy.QuerySelector("img").GetAttributeValue("src", "")
                                : nodeCopy.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");

                            var name = $"{nodeCopy.QuerySelector("h3").InnerText}";
                            var authorName = nodeCopy.QuerySelector("span[class=\"author\"]").InnerText;
                            var id = $"{_regex05.Match(nodeCopy.QuerySelector("h3 > a").GetAttributeValue("href", "")).Groups[1].Value}";
                            var numberOfChapter = int.Parse(_regex09.Match(nodeCopy.QuerySelectorAll(".author")[^1].InnerText).Groups[0].Value);
                            return new Story(name, id, imageLink, authorName, numberOfChapter);
                        }));
                    }
                    Task.WaitAll(nodeTasks.ToArray());
                    return nodeTasks.Select(nt => nt.Result).ToList();
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                result.AddRange(task.Result);
            }
            return result;
        }
        catch
        {
            throw new System.Exception("some thing wrong");
        }
    }

    public PagedList<Story> GetStoriesOfCategory(string categoryId, int page, int limit)
    {
        if (page < 1 || limit < 0)
        {
            throw new System.Exception("invalid paging data");
        }

        try
        {
            var skippedElements = limit * (page - 1);
            var startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
            var startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
            var totalStory = GetTotalStoryCategory(categoryId);
            var totalPage = (int)Math.Ceiling((double)totalStory / limit);
            var stories = new List<Story>();
            if (skippedElements > totalStory)
            {
                return new PagedList<Story>(page, limit, totalPage, stories);
            }

            var count = 0;
            while (count < limit)
            {
                if (startPage >= totalPage)
                {
                    break;
                }

                var doc = LoadHtmlDocument($"{HOME_URL}/the-loai/{categoryId}/trang-{startPage + 1}");
                var nodes = doc.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").ToList();
                if (startIndex >= nodes.Count())
                {
                    break;
                }

                for (; startIndex < nodes.Count; startIndex++)
                {
                    count++;
                    if (count > limit || count > totalStory || startIndex >= nodes.Count())
                    {
                        break;
                    }

                    var node = nodes[startIndex];
                    var imageLink = node.QuerySelectorAll("img").Count() != 0
                        ? node.QuerySelector("img").GetAttributeValue("src", "")
                        : node.QuerySelector(".lazyimg").GetAttributeValue("data-image", "");
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
        catch
        {
            throw new System.Exception("some thing wrong");

        }
    }

    public StoryDetail GetStoryDetail(string storyId)
    {
        try
        {
            var story = GetExactStory(storyId);
            var doc = LoadHtmlDocument($"{HOME_URL}{storyId}");
            var dataNode = doc.QuerySelector(".text-base");
            var authorNode = doc.QuerySelector("a[itemprop=\"author\"]");
            var author = new Author(authorNode.InnerText, _regex07.Match(authorNode.GetAttributeValue("href", "")).Groups[1].Value);
            var infoNodes = doc.QuerySelectorAll(".info > div");
            var categoriesNode = infoNodes[1].QuerySelectorAll("a");
            var categories = new List<Category>();
            foreach (var category in categoriesNode)
            {
                if (category.InnerText == "Tất cả" || category.InnerText == "Khác")
                {
                    continue;
                }

                categories.Add(new Category(category.InnerText, _regex01.Match(category.GetAttributeValue("href", "")).Groups[1].Value));
            }
            var status = infoNodes[infoNodes.Count - 1].QuerySelector("span").InnerText;
            var description = doc.QuerySelector("div[itemprop=\"description\"]").InnerHtml.Replace("<br>", "\n");
            description = description.Replace("<br>", "\n");
            description = Regex.Replace(description, @"<.*?>", string.Empty);
            return new StoryDetail(story, author, status, categories.ToArray(), description);
        }
        catch
        {
            throw new System.Exception("some thing wrong");
        }

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
        if (authorName.Length == 0)
        {
            throw new System.Exception("Invalid author name");
        }
        var tasks = new List<Task<List<Author>>>();
        var result = new List<Author>();
        var searchContent = WebUtility.UrlDecode(authorName);
        var doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}");
        var pagingBtn = doc.QuerySelectorAll(".pagination > li > a");
        var lastIndex = pagingBtn.Count > 0 ? int.Parse(_regex04.Match(pagingBtn[pagingBtn.Count - 1].GetAttributeValue("href", "")).Groups[1].Value) : 1;
        for (var i = 1; i <= lastIndex; i++)
        {
            var iCopy = i;
            tasks.Add(Task.Run(() =>
            {
                var authors = new List<Author>();
                var curPage = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}&page={iCopy}");
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
        if (authorName.Length == 0)
        {
            throw new System.Exception("Invalid author name");
        }
        var searchContent = WebUtility.UrlDecode(authorName);
        var skippedElements = limit * (page - 1);
        var startPage = (int)Math.Floor((double)skippedElements / DEFAULT_SEARCH_SIZE);
        var startIndex = skippedElements % DEFAULT_SEARCH_SIZE;
        var totalStory = GetTotalStorySearch(searchContent);
        var totalPage = (int)Math.Ceiling((double)totalStory / limit);
        var authors = new List<Author>();
        if (skippedElements > totalStory)
        {
            return new PagedList<Author>(page, limit, totalPage, authors);
        }

        var count = 0;
        while (count < limit)
        {
            if (startPage >= totalPage)
            {
                break;
            }

            var doc = LoadHtmlDocument($"{HOME_URL}/tim-kiem/?tukhoa={searchContent}&page={startPage + 1}");
            var nodes = doc.QuerySelectorAll("div[itemtype=\"https://schema.org/Book\"]").ToList();
            if (startIndex >= nodes.Count())
            {
                break;
            }

            for (; startIndex < nodes.Count; startIndex++)
            {
                count++;
                if (count > limit || count > totalStory || startIndex >= nodes.Count())
                {
                    break;
                }

                var node = nodes[startIndex];
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
        var count = GetTotalChap(storyId);
        return count == -1 ? throw new System.Exception("invalid story id") : count;
    }

    public PagedList<Story> GetStoriesBySearchNameWithFilter(string storyName, int minChapNum, int maxChapNum, int page, int limit)
    {
        if (storyName.Length == 0)
        {
            throw new System.Exception("Invalid author name");
        }
        var stories = GetStoriesBySearchName(storyName);
        var filterStories = stories.Where(story => story.NumberOfChapter >= minChapNum && story.NumberOfChapter <= maxChapNum).ToList();
        var totalStory = filterStories.Count;
        var totalPage = (int)Math.Ceiling((double)totalStory / limit);
        filterStories = filterStories.Skip((page - 1) * limit).Take(limit).ToList();
        return new PagedList<Story>(page, limit, totalPage, filterStories);
    }
}
