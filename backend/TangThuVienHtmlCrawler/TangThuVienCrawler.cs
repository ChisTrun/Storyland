using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json;
using PluginBase.Contract;
using PluginBase.Models;
using PluginBase.Utils;
using System.Data.SqlTypes;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Web;

namespace TangThuVienHtmlCrawler;

public class TangThuVienCrawler : ICrawler
{
    protected static HtmlDocument GetWebPageDocument(string sourceURL)
    {
        sourceURL = HttpUtility.UrlDecode(sourceURL);

        var web = new HtmlWeb();
        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
        var document = web.Load(sourceURL);
        return document;
    }

    // authorUrl: https://truyen.tangthuvien.vn/tac-gia?author=65
    // storyUrl: https://truyen.tangthuvien.vn/doc-truyen/gia-toc-tu-tien-tong-thi-truong-thanh
    // chapterUrl: https://truyen.tangthuvien.vn/doc-truyen/gia-toc-tu-tien-tong-thi-truong-thanh/chuong-1

    public static string Domain => "https://truyen.tangthuvien.vn";
    public static string DomainTongHop => $"{Domain}/tong-hop";
    public static string DomainTimKiem => $"{Domain}/tim-kiem";
    public static string DomainKetQuaTimKiem => $"{Domain}/ket-qua-tim-kiem";
    public static string DomainDocTruyen => $"{Domain}/doc-truyen";
    public static string DomainTacGia => $"{Domain}/tac-gia";

    public string Name => "Tàng Thư Viện";

    public string Description => "";

    public IEnumerable<Category> GetCategories()
    {
        var categories = new List<Category>();
        var document = GetWebPageDocument(DomainTongHop);
        var categoriesSelector = @"body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.rank-header > div > div > div > div > p > a[data-value]";
        var categoryATags = document.QuerySelectorAll(categoriesSelector);
        string GetCategoryUrl(string cateId) => $"{DomainTongHop}?ctg={cateId}";
        foreach (var categoryATag in categoryATags)
        {
            var id = categoryATag.GetDataAttribute("value").Value;
            var url = GetCategoryUrl(id);
            var name = categoryATag.GetDirectInnerTextDecoded();
            categories.Add(new Category(name, ModelExtension.GetIDFromUrl(ModelType.Category, url)));
        }
        return categories;
    }

    // https://truyen.tangthuvien.vn/tong-hop?ctg=1&limit=100000
    public IEnumerable<Story> GetStoriesOfCategory(string categoryId)
    {
        string categoryUrl = ModelExtension.GetUrlFromID(ModelType.Category, categoryId);
        var baseUrl = $"{categoryUrl}&limit=10000";
        var document = GetWebPageDocument(baseUrl);
        var stories = RankViewListFormat.CrawlStoriesFromAPage(document);
        return stories;
    }

    // https://truyen.tangthuvien.vn/tong-hop?ctg=1&page=800&limit=2
    public IEnumerable<Story> GetStoriesOfCategory(string categoryId, int page, int limit)
    {
        string categoryUrl = ModelExtension.GetUrlFromID(ModelType.Category, categoryId);
        var baseUrl = $"{categoryUrl}&limit={limit}&page={page}";
        var document = GetWebPageDocument(baseUrl);
        var stories = RankViewListFormat.CrawlStoriesFromAPage(document);
        return stories;
    }

    // https://truyen.tangthuvien.vn/ket-qua-tim-kiem?term=dinh&page=2
    public IEnumerable<Story> GetStoriesBySearchName(string storyName)
    {
        var baseUrl = $"{DomainKetQuaTimKiem}?term={WebUtility.UrlEncode(storyName)}";
        var createUrlFromPage = new CreateURLFromPage((page) => $"{baseUrl}&page={page}");
        IEnumerable<Story> stories = GetRepresentativesFromATagsFromAllPages(RankViewListFormat.CrawlStoriesFromAPage, createUrlFromPage.Invoke, RankViewListFormat.IsLastPage);
        return stories;
    }

    // https://truyen.tangthuvien.vn/ket-qua-tim-kiem?term=dinh&page=2
    // dont have limit
    public IEnumerable<Story> GetStoriesBySearchName(string storyName, int page, int limit)
    {
        var baseUrl = $"{DomainKetQuaTimKiem}?term={WebUtility.UrlEncode(storyName)}";
        var createUrlFromPage = new CreateURLFromPage((page) => $"{baseUrl}&page={page}");
        IEnumerable<Story> stories = GetRepresentativesFromATagsFromAllPagesFixedPageSize(RankViewListFormat.CrawlStoriesFromAPage, createUrlFromPage.Invoke, RankViewListFormat.IsLastPage, page, limit);
        return stories;
    }

    // https://truyen.tangthuvien.vn/tac-gia?author=27&page=1
    public IEnumerable<Story> GetStoriesOfAuthor(string authorId)
    {
        var authorUrl = ModelExtension.GetUrlFromID(ModelType.Author, authorId);
        var createUrlFromPage = new CreateURLFromPage((page) => $"{authorUrl}&page={page}");
        IEnumerable<Story> stories = GetRepresentativesFromATagsFromAllPages(RankViewListFormat.CrawlStoriesFromAPage, createUrlFromPage.Invoke, RankViewListFormat.IsLastPage);
        return stories;
    }

    // https://truyen.tangthuvien.vn/tac-gia?author=27&page=1
    // dont have limit
    public IEnumerable<Story> GetStoriesOfAuthor(string authorId, int page, int limit)
    {
        var authorUrl = ModelExtension.GetUrlFromID(ModelType.Author, authorId);
        var createUrlFromPage = new CreateURLFromPage((page) => $"{authorUrl}&page={page}");
        IEnumerable<Story> stories = GetRepresentativesFromATagsFromAllPagesFixedPageSize(RankViewListFormat.CrawlStoriesFromAPage, createUrlFromPage.Invoke, RankViewListFormat.IsLastPage, page, limit);
        return stories;
    }

    // https://truyen.tangthuvien.vn/doc-truyen/page/38020?page=0&limit=100000&web=1
    public List<Chapter> GetChaptersOfStory(string storyId)
    {
        var story = GetExactStory(storyId);
        var id = GetWebPageDocument(story.GetUrl()).QuerySelector("#story_id_hidden").GetAttributeValue("value", null) ?? throw new Exception();
        var chapters = new List<Chapter>();
        var chaptersUrl = $"{DomainDocTruyen}/page/{id}?page=0&limit=100000&web=1";
        // watch it content, it not always return the same as browser render
        var chaptersSelector = @"ul > li > a";
        var document = GetWebPageDocument(chaptersUrl);
        var aTags = document.QuerySelectorAll(chaptersSelector);
        var count = 0;
        foreach (var aTag in aTags)
        {
            var fontTag = aTag.QuerySelector("font > font");
            if (fontTag != null)
            {
                var url = aTag.GetAttributeValue("href", null) ?? throw new Exception();
                var name = fontTag.GetDirectInnerTextDecoded();
                chapters.Add(new Chapter(name, ModelExtension.GetIDFromUrl(ModelType.Chapter, url), story, count));
                count++;
            }
        }
        return chapters;
    }

    // https://truyen.tangthuvien.vn/doc-truyen/page/38020?page=0&limit=100000&web=1
    public List<Chapter> GetChaptersOfStory(string storyId, int page, int limit)
    {
        var story = GetExactStory(storyId);
        var id = GetWebPageDocument(story.GetUrl()).QuerySelector("#story_id_hidden").GetAttributeValue("value", null) ?? throw new Exception();
        var chapters = new List<Chapter>();
        var chaptersUrl = $"{DomainDocTruyen}/page/{id}?page={page - 1}&limit={limit}&web=1";
        // watch it content, it not always return the same as browser render
        var chaptersSelector = @"ul > li > a";
        var document = GetWebPageDocument(chaptersUrl);
        var aTags = document.QuerySelectorAll(chaptersSelector);
        var count = 0;
        foreach (var aTag in aTags)
        {
            var fontTag = aTag.QuerySelector("font > font");
            if (fontTag != null)
            {
                var url = aTag.GetAttributeValue("href", null) ?? throw new Exception();
                var name = fontTag.GetDirectInnerTextDecoded();
                chapters.Add(new Chapter(name, ModelExtension.GetIDFromUrl(ModelType.Chapter, url), story, count));
                count++;
            }
        }
        return chapters;
    }

    // https://truyen.tangthuvien.vn/doc-truyen/trong-sinh-chi-vu-em-nhan-nha-sinh-hoat/chuong-480
    public ChapterContent GetChapterContent(string storyId, int index)
    {
        return GetChapterContent($"{storyId}/chuong-{index}");
    }

    // https://truyen.tangthuvien.vn/doc-truyen/trong-sinh-chi-vu-em-nhan-nha-sinh-hoat/chuong-480
    public ChapterContent GetChapterContent(string chapterId)
    {
        var chapterUrl = ModelExtension.GetUrlFromID(ModelType.Chapter, chapterId);
        var document = GetWebPageDocument(chapterUrl);
        var contentSelector = "div.chapter-c > div.chapter-c-content > div.box-chap:not(.hidden)";
        var content = document.QuerySelector(contentSelector).GetDirectInnerTextDecoded();
        int total;
        {
            var storyId = document.QuerySelector("body > div.box-report > form > input[name=story_id]").GetAttributeValue("value", null);
            var chaptersUrl = $"https://truyen.tangthuvien.vn/story/chapters?story_id={storyId}";
            var documentChapters = GetWebPageDocument(chaptersUrl);
            total = int.Parse(documentChapters.QuerySelector("ul > li:last-child").GetAttributeValue("title", null) ?? throw new Exception());
        }
        var splitIndex = chapterId.LastIndexOf('-');
        var chapSplit = chapterId[..(splitIndex + 1)];
        var indexSplit = chapterId[(splitIndex + 1)..];
        var current = int.Parse(indexSplit);
        var prevChapIndex = Math.Max(0, current - 1);
        var nextChapIndex = Math.Min(total, current + 1);
        var prevChapId = $"{chapSplit}{prevChapIndex}";
        var nextChapId = $"{chapSplit}{nextChapIndex}";
        return new ChapterContent(WebUtility.HtmlEncode(content), nextChapId, prevChapId);
    }

    // https://truyen.tangthuvien.vn/doc-truyen/dichdinh-cao-quyen-luc-suu-tam
    public StoryDetail GetStoryDetail(string storyId)
    {
        var story = GetExactStory(storyId);
        var url = story.GetUrl();
        var document = GetWebPageDocument(url);
        var authorATag = document.QuerySelector("body > div.book-detail-wrap.center990 > div.book-information.cf > div.book-info > p.tag > a.blue");
        var tuple = GetNameUrlFromATag(authorATag);
        var author = new Author(tuple.Item2, ModelExtension.GetIDFromUrl(ModelType.Author, tuple.Item1));
        var status = document.QuerySelector("body > div.book-detail-wrap.center990 > div.book-information.cf > div.book-info > p.tag > span").GetDirectInnerTextDecoded();
        var categoryTag = document.QuerySelector("body > div.book-detail-wrap.center990 > div.book-information.cf > div.book-info > p.tag > a.red");
        var categorySubUrl = categoryTag.GetAttributeValue("href", null);
        var category = GetCategoryFromSubURL(categorySubUrl);
        var descriptionPTag = document.QuerySelector("body > div.book-detail-wrap.center990 > div.book-content-wrap.cf > div.left-wrap.fl > div.book-info-detail > div.book-intro > p");
        var description = descriptionPTag.GetDirectInnerTextDecoded();
        return new StoryDetail(story, author, status, [category], description);
    }

    // ================= End Interface ====================
    // ================= End Interface ====================
    // ================= End Interface ====================

    // #rank-list-view format
    // #rank-list-view format
    // #rank-list-view format

    private delegate string CreateURLFromPage(int page);
    private delegate Representative CreateDefaultRepresentative(string name, string url);

    private static IEnumerable<T> GetRepresentativesFromATagsFromAllPages<T>(Func<HtmlDocument, IEnumerable<T>> crawlRepresentativesFromAPage, Func<int, string> createURLFromPage, Predicate<HtmlDocument> nextPageAvailible) where T : Representative
    {
        var page = 1;
        var tasks = new List<Task<IEnumerable<T>>>();
        while (true)
        {
            var accessUrl = createURLFromPage.Invoke(page);
            var document = GetWebPageDocument(accessUrl);
            tasks.Add(Task.Run(() =>
            {
                var results = crawlRepresentativesFromAPage.Invoke(document);
                return results;
            }));
            if (nextPageAvailible.Invoke(document) == true)
            {
                break;
            }
            page += 1;
        }
        var resultTask = Task.WhenAll(tasks);
        resultTask.Wait();
        var representativesArray = resultTask.Result;
        var representatives = representativesArray.SelectMany(x => x).ToList();
        return representatives;
    }

    private static IEnumerable<T> GetRepresentativesFromATagsFromAllPagesFixedPageSize<T>(Func<HtmlDocument, IEnumerable<T>> crawlRepresentativesFromAPage, Func<int, string> createURLFromPage, Predicate<HtmlDocument> nextPageAvailible, int page, int limit) where T : Representative
    {
        int systemLimit;
        {
            var document = GetWebPageDocument(createURLFromPage.Invoke(0));
            systemLimit = document.QuerySelectorAll("#rank-view-list > div > ul > li").Count;
        }
        int systemPageStart;
        int systemPageEnd;
        int systemOffsetStart;
        int systemOffsetEnd;
        {
            var offsetStart = (page - 1) * limit;
            var offsetEnd = (page * limit) - 1;
            systemPageStart = offsetStart / systemLimit;
            systemOffsetStart = offsetStart % systemLimit;
            systemPageEnd = offsetEnd / systemLimit;
            systemOffsetEnd = offsetEnd % systemLimit;
        }
        int systemCurrentPage = systemPageStart;
        var results = new List<T>();
        while (systemCurrentPage <= systemPageEnd)
        {
            var accessUrl = createURLFromPage.Invoke(systemCurrentPage);
            var document = GetWebPageDocument(accessUrl);
            var pageResults = crawlRepresentativesFromAPage.Invoke(document);
            var offsetStart = systemCurrentPage == systemPageStart ? systemOffsetStart : 0;
            var offsetEnd = systemCurrentPage == systemPageEnd ? systemOffsetEnd : systemLimit - 1;
            results.AddRange(pageResults.Skip(offsetStart).Take(offsetEnd - offsetStart + 1));
            if (nextPageAvailible.Invoke(document) == true)
            {
                break;
            }
            systemCurrentPage += 1;
        }
        return results;
    }

    // sub-methods
    // sub-methods
    // sub-methods

    private Tuple<string, string> GetNameUrlFromATag(HtmlNode aTag)
    {
        var url = aTag.GetAttributeValue("href", null) ?? throw new Exception();
        var name = aTag.GetDirectInnerTextDecoded();
        return Tuple.Create(url, name);
    }

    private Story GetExactStory(string id)
    {
        var doc = GetWebPageDocument(ModelExtension.GetUrlFromID(ModelType.Story, id));
        var name = doc.QuerySelector("head > title").GetDirectInnerTextDecoded();
        var imgUrl = doc.QuerySelector("#bookImg > img").GetAttributeValue("src", null);
        return new Story(name, id, imgUrl);
    }

    // https://truyen.tangthuvien.vn/the-loai/huyen-huyen
    // https://truyen.tangthuvien.vn/tong-hop?ctg=2
    private Category GetCategoryFromSubURL(string subUrl)
    {
        var doc = GetWebPageDocument(subUrl);
        var name = doc.QuerySelector("head > title").GetDirectInnerTextDecoded();
        var urlRaw = doc.QuerySelector("#update-tab > a").GetAttributeValue("href", null);
        int startIndex = urlRaw.IndexOf("ctg=");
        string ctgSubstring = urlRaw[startIndex..];
        int endIndex = ctgSubstring.IndexOf('&');
        if (endIndex == -1)
        {
            endIndex = ctgSubstring.Length;
        }
        string ctgValue = ctgSubstring[..endIndex];
        return new Category(name, "?" + ctgValue);
    }

    private Category GetExactCategory(string id)
    {
        var doc = GetWebPageDocument(ModelExtension.GetUrlFromID(ModelType.Category, id));
        var name = doc.QuerySelector("body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.rank-header > div > div > div > div > p:nth - child(1) > a.act").GetDirectInnerTextDecoded();
        return new Category(name, id);
    }

    // Extra functions for later uses
    // Extra functions for later uses
    // Extra functions for later uses

    // note: search engine của trang này không được ổn định
    public IEnumerable<Author> GetAuthorsBySearchName(string name)
    {
        var authors = new List<Author>();
        var domainTimKiemWithKey = $"{DomainTimKiem}?term={WebUtility.UrlEncode(name)}";
        Task.Run(async () =>
        {
            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync(domainTimKiemWithKey);
                response.EnsureSuccessStatusCode();
                string jsonData = await response.Content.ReadAsStringAsync();
                var jsonTypeList = new[] { new { id = 0, name = "", url = "", type = "", story_type = 0 } };
                var resArray = JsonConvert.DeserializeAnonymousType(jsonData, jsonTypeList) ?? throw new Exception("Null response");
                foreach (var res in resArray)
                {
                    // story_type == 1 leads to ngontinh.tangthuvien
                    if (res.type == "author")
                    {
                        var url = $"{DomainTacGia}?author={res.id}";
                        authors.Add(new Author(res.name, ModelExtension.GetIDFromUrl(ModelType.Author, url)));
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                throw;
            }
        }).Wait();
        return authors;
    }

    internal static class RankViewListFormat
    {
        public static IEnumerable<Story> CrawlStoriesFromAPage(HtmlDocument doc)
        {
            var framesSelector = "#rank-view-list > div > ul > li";
            var aTagSelector = "div.book-mid-info > h4 > a";
            var imageSelector = "div.book-img-box > a > img";
            var frames = doc.QuerySelectorAll(framesSelector);
            var stories = new List<Story>();
            foreach (var frame in frames)
            {
                var aTag = frame.QuerySelector(aTagSelector);
                var image = frame.QuerySelector(imageSelector);
                var url = aTag.GetAttributeValue("href", null) ?? throw new Exception();
                var name = aTag.GetDirectInnerTextDecoded();
                var imageUrl = image.GetAttributeValue("src", null);
                stories.Add(new Story(name, ModelExtension.GetIDFromUrl(ModelType.Story, url), imageUrl));
            }
            return stories;
        }

        public static bool IsLastPage(HtmlDocument doc)
        {
            var liAbleNextPageSelector = "body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.page-box.cf > div > ul > li:not(.disabled):last-child";
            return doc.QuerySelector(liAbleNextPageSelector) == null;
        }
    }
}
