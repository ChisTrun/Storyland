﻿using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json;
using PluginBase.Contract;
using PluginBase.Models;
using PluginBase.Utils;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace TangThuVien;

public partial class TangThuVienCrawler : ICrawler
{

    protected static HtmlDocument GetWebPageDocument(string sourceURL)
    {
        sourceURL = HttpUtility.UrlDecode(sourceURL);
        var web = new HtmlWeb();
        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
        var document = web.Load(sourceURL);
        var count = 0;
        while (true)
        {
            var title = document.QuerySelector("title");
            if (title != null && title.GetDirectInnerTextDecoded() == "Site Maintenance")
            {
                if (count > 10)
                {
                    throw new Exception("Bad request");
                }
                count++;
                document = web.Load(sourceURL);
            }
            else
            {
                break;
            }
        }
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
    public static string DomainTheLoai => $"{Domain}/the-loai";

    public string Name => "Tàng Thư Viện";

    public string Description => "";

    private string CategoryIDCast(string orgID)
    {
        var url = $"{DomainTheLoai}/{orgID}";
        var doc = GetWebPageDocument(url);
        var xemThem = doc.QuerySelector("#update-tab > a");
        var xemThemURL = xemThem.GetAttributeValue("href", null) ?? throw new Exception();
        var id = GetCTG().Match(xemThemURL).Value;
        return "?" + id;
    }

    public IEnumerable<Category> GetCategories()
    {
        var categories = new List<Category>();
        var document = GetWebPageDocument(Domain);
        var rawCategories = document.QuerySelectorAll("#classify-list > dl > dd > a");
        foreach (var rawCate in rawCategories)
        {
            if (rawCate.QuerySelector("cite > span > b") != null)
            {
                var name = rawCate.QuerySelector("cite > span > i").GetDirectInnerTextDecoded();
                var url = rawCate.GetAttributeValue("href", null);
                categories.Add(new Category(name, url.TakeLastParamURL()));
            }
        }
        return categories;
    }

    // https://truyen.tangthuvien.vn/tong-hop?ctg=1&limit=100000
    public IEnumerable<Story> GetStoriesOfCategory(string categoryId)
    {
        string castId = CategoryIDCast(categoryId);
        string categoryUrl = ModelExtension.GetUrlFromID(ModelType.Category, castId);
        var baseUrl = $"{categoryUrl}&limit=10000";
        var document = GetWebPageDocument(baseUrl);
        var stories = RankViewListFormat.CrawlStoriesFromAPage(document);
        return stories;
    }

    // https://truyen.tangthuvien.vn/tong-hop?ctg=1&page=800&limit=2
    public PagingRepresentative<Story> GetStoriesOfCategory(string categoryId, int page, int limit)
    {
        string categoryUrl = ModelExtension.GetUrlFromID(ModelType.Category, CategoryIDCast(categoryId));
        var baseUrl = $"{categoryUrl}&limit={limit}&page={page}";
        var document = GetWebPageDocument(baseUrl);
        var stories = RankViewListFormat.CrawlStoriesFromAPage(document);
        var lastLiTag = document.QuerySelector("body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.page-box.cf > div > ul > li:last-child");
        var aTag = lastLiTag.PreviousSiblingElement().GetChildElements().First();
        var totalPage = int.Parse(aTag.GetDirectInnerTextDecoded());
        var paging = new PagingRepresentative<Story>(page, limit, totalPage, stories);
        return paging;
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
    public PagingRepresentative<Story> GetStoriesBySearchName(string storyName, int page, int limit)
    {
        var baseUrl = $"{DomainKetQuaTimKiem}?term={WebUtility.UrlEncode(storyName)}";
        var createUrlFromPage = new CreateURLFromPage((page) => $"{baseUrl}&page={page}");
        var pagingStories = GetRepresentativesFromATagsFromAllPagesFixedPageSize(RankViewListFormat.CrawlStoriesFromAPage, createUrlFromPage.Invoke, RankViewListFormat.IsLastPage, page, limit);
        return pagingStories;
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
    public PagingRepresentative<Story> GetStoriesOfAuthor(string authorId, int page, int limit)
    {
        var authorUrl = ModelExtension.GetUrlFromID(ModelType.Author, authorId);
        var createUrlFromPage = new CreateURLFromPage((page) => $"{authorUrl}&page={page}");
        var pagingStories = GetRepresentativesFromATagsFromAllPagesFixedPageSize(RankViewListFormat.CrawlStoriesFromAPage, createUrlFromPage.Invoke, RankViewListFormat.IsLastPage, page, limit);
        return pagingStories;
    }

    // https://truyen.tangthuvien.vn/doc-truyen/page/38020?page=0&limit=100000&web=1
    public IEnumerable<Chapter> GetChaptersOfStory(string storyId)
    {
        var story = GetStory(storyId);
        var ttvStoryId = GetTTVStoryId(storyId);
        var chapters = new List<Chapter>();
        var chaptersUrl = $"{DomainDocTruyen}/page/{ttvStoryId}?page=0&limit=100000&web=1";
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

    // https://truyen.tangthuvien.vn/doc-truyen/page/6270?page=2&limit=10&web=1
    // https://truyen.tangthuvien.vn/story/chapters?story_id=6270
    public PagingRepresentative<Chapter> GetChaptersOfStory(string storyId, int page, int limit)
    {
        var story = GetStory(storyId);
        var ttvStoryId = GetTTVStoryId(storyId);
        var chapters = new List<Chapter>();
        var chaptersUrl = $"{DomainDocTruyen}/page/{ttvStoryId}?page={page - 1}&limit={limit}&web=1";
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
        var documentTotal = GetWebPageDocument($"https://truyen.tangthuvien.vn/story/chapters?story_id={ttvStoryId}");
        var totalRecord = documentTotal.QuerySelectorAll("ul > li").Count;
        var totalPage = (totalRecord / limit) + (totalRecord % limit == 0 ? 0 : 1);
        return new PagingRepresentative<Chapter>(page, limit, totalPage, chapters);
    }

    // https://truyen.tangthuvien.vn/doc-truyen/trong-sinh-chi-vu-em-nhan-nha-sinh-hoat/chuong-480
    public ChapterContent GetChapterContent(string storyId, int index)
    {
        var currentChapterIndex = index + 1;
        var ttvStoryId = GetTTVStoryId(storyId);
        var chaptersUrl = $"https://truyen.tangthuvien.vn/story/chapters?story_id={ttvStoryId}";
        var chaptersDoc = GetWebPageDocument(chaptersUrl);
        var total = chaptersDoc.QuerySelector(@"ul:last-child > li:last-child").GetAttributeValue("title", -1);
        if (currentChapterIndex < 1 || currentChapterIndex > total)
        {
            throw new Exception();
        }
        var chapterUrl = chaptersDoc.QuerySelector($"a.link-chap-{currentChapterIndex}").GetAttributeValue("href", null);
        var chapterId = ModelExtension.GetIDFromUrl(ModelType.Chapter, chapterUrl);
        var chapterDoc = GetWebPageDocument(chapterUrl);
        GetNameContentStoryOfChapter(chapterDoc, out string chapterName, out string content, out Story story);
        PrevNextChapId(chaptersDoc, currentChapterIndex, out string? prevChapId, out string? nextChapId);
        return new ChapterContent(WebUtility.HtmlEncode(content), nextChapId, prevChapId, chapterName, chapterId, currentChapterIndex - 1, story);
    }

    // storyId + index => chapterId (90% accurate)
    // storyURL: https://truyen.tangthuvien.vn/doc-truyen/trong-sinh-chi-vu-em-nhan-nha-sinh-hoat
    // chapterId: https://truyen.tangthuvien.vn/doc-truyen/trong-sinh-chi-vu-em-nhan-nha-sinh-hoat/chuong-480
    // other storyId: https://truyen.tangthuvien.vn/story/chapters?story_id=38020
    public ChapterContent GetChapterContent(string chapterId)
    {
        var chapterUrl = ModelExtension.GetUrlFromID(ModelType.Chapter, chapterId);
        var chapterDoc = GetWebPageDocument(chapterUrl);
        var script = chapterDoc.QuerySelector(@"body > div.container.body-container > div.content > div > script").InnerHtml;
        var ttvChapterUrl = Regex.Match(script, @"https:\/\/truyen\.tangthuvien\.vn\/story\/chapters\?story_id=37297&chapter_id=\d+").Value;
        var docChaptersWithActive = GetWebPageDocument(ttvChapterUrl);
        var currentChapterIndex = docChaptersWithActive.QuerySelector(@"li.active").GetAttributeValue("title", -1);
        GetNameContentStoryOfChapter(chapterDoc, out string chapterName, out string content, out var story);
        PrevNextChapId(docChaptersWithActive, currentChapterIndex, out string? prevChapId, out string? nextChapId);
        return new ChapterContent(WebUtility.HtmlEncode(content), nextChapId, prevChapId, chapterName, chapterId, currentChapterIndex - 1, story);
    }

    // https://truyen.tangthuvien.vn/doc-truyen/dichdinh-cao-quyen-luc-suu-tam
    public StoryDetail GetStoryDetail(string storyId)
    {
        var story = GetStory(storyId);
        var url = ModelExtension.GetUrlFromID(ModelType.Story, storyId);
        var document = GetWebPageDocument(url);
        var authorATag = document.QuerySelector("body > div.book-detail-wrap.center990 > div.book-information.cf > div.book-info > p.tag > a.blue");
        var tuple = GetNameUrlFromATag(authorATag);
        var author = new Author(tuple.Item2, ModelExtension.GetIDFromUrl(ModelType.Author, tuple.Item1));
        var status = document.QuerySelector("body > div.book-detail-wrap.center990 > div.book-information.cf > div.book-info > p.tag > span").GetDirectInnerTextDecoded();
        var categoryTag = document.QuerySelector("body > div.book-detail-wrap.center990 > div.book-information.cf > div.book-info > p.tag > a.red");
        var id = categoryTag.GetAttributeValue("href", null).TakeLastParamURL();
        var name = categoryTag.GetDirectInnerTextDecoded();
        var category = new Category(name, id);
        var descriptionPTag = document.QuerySelector("body > div.book-detail-wrap.center990 > div.book-content-wrap.cf > div.left-wrap.fl > div.book-info-detail > div.book-intro > p");
        var description = descriptionPTag.GetDirectInnerTextDecoded();
        return new StoryDetail(story, author, status, [category], description);
    }

    // note: search engine cua trang nay khong duoc on dinh
    public IEnumerable<Author> GetAuthorsBySearchName(string authorName)
    {
        var authors = new List<Author>();
        var domainTimKiemWithKey = $"{DomainTimKiem}?term={WebUtility.UrlEncode(authorName)}";
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

    private static PagingRepresentative<T> GetRepresentativesFromATagsFromAllPagesFixedPageSize<T>(Func<HtmlDocument, IEnumerable<T>> crawlRepresentativesFromAPage, Func<int, string> createURLFromPage, Predicate<HtmlDocument> nextPageAvailible, int page, int limit) where T : Representative
    {
        int systemLimit;
        int totalPage;
        int totalRecord;
        {
            var firstURL = createURLFromPage.Invoke(0);
            var documentFirstPage = GetWebPageDocument(firstURL);
            systemLimit = documentFirstPage.QuerySelectorAll("#rank-view-list > div > ul > li > div.book-mid-info").Count;
            if (systemLimit == 0)
            {
                return new PagingRepresentative<T>(page, limit, 0, new List<T>());
            }
            var lastLiPagingTag = documentFirstPage.QuerySelector("body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.page-box.cf > div > ul > li:last-child");
            if (lastLiPagingTag == null)
            {
                totalRecord = systemLimit;
            }
            else
            {
                var aTag = lastLiPagingTag.PreviousSiblingElement().GetChildElements().First();
                var systemTotalPage = int.Parse(aTag.GetDirectInnerTextDecoded());
                var lastURL = createURLFromPage(systemTotalPage);
                var documentLastPage = GetWebPageDocument(lastURL);
                var systemLastPageCount = documentLastPage.QuerySelectorAll("#rank-view-list > div > ul > li").Count;
                totalRecord = systemLimit * (systemTotalPage - 1) + systemLastPageCount;
            }
            totalPage = (totalRecord / limit) + (totalRecord % limit == 0 ? 0 : 1);
        }
        int systemPageStart;
        int systemPageEnd;
        int systemOffsetStart;
        int systemOffsetEnd;
        {
            var offsetStart = (page - 1) * limit;
            var offsetEnd = Math.Min((page * limit) - 1, totalRecord - 1);
            systemPageStart = (offsetStart / systemLimit) + 1;
            systemOffsetStart = offsetStart % systemLimit;
            systemPageEnd = (offsetEnd / systemLimit) + 1;
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
        return new PagingRepresentative<T>(page, limit, totalPage, results);
    }

    // sub-methods
    // sub-methods
    // sub-methods

    private static Tuple<string, string> GetNameUrlFromATag(HtmlNode aTag)
    {
        var url = aTag.GetAttributeValue("href", null) ?? throw new Exception();
        var name = aTag.GetDirectInnerTextDecoded();
        return Tuple.Create(url, name);
    }

    // https://truyen.tangthuvien.vn/doc-truyen/thi-ra-ho-moi-la-nhan-vat-chinh
    // differ from the CrawlStoriesFromAPage
    private static Story GetStory(string id)
    {
        var doc = GetWebPageDocument(ModelExtension.GetUrlFromID(ModelType.Story, id));
        var name = doc.QuerySelector("head > title").GetDirectInnerTextDecoded();
        var imgUrl = doc.QuerySelector("#bookImg > img").GetAttributeValue("src", null);
        var authorName = doc.QuerySelector(@"body > div.book-detail-wrap.center990 > div.book-information.cf > div.book-info > p.tag > a.blue").GetDirectInnerTextDecoded();
        return new Story(name, id, imgUrl, authorName);
    }

    private static string GetTTVStoryId(string storyId)
    {
        var storyUrl = ModelExtension.GetUrlFromID(ModelType.Story, storyId);
        var id = GetWebPageDocument(storyUrl).QuerySelector("#story_id_hidden").GetAttributeValue("value", null) ?? throw new Exception();
        return id;
    }

    private static void PrevNextChapId(HtmlDocument docChapters, int currentChapterIndex, out string? prevChapId, out string? nextChapId)
    {
        var total = docChapters.QuerySelector(@"ul:last-child > li:last-child").GetAttributeValue("title", -1);
        prevChapId = null;
        nextChapId = null;
        var prevChapterIndex = currentChapterIndex - 1;
        if (prevChapterIndex >= 1)
        {
            var prevChapUrl = docChapters.QuerySelector($"a.link-chap-{prevChapterIndex}").GetAttributeValue("href", null).Trim();
            prevChapId = ModelExtension.GetIDFromUrl(ModelType.Chapter, prevChapUrl).Trim();
        }
        var nextChapterIndex = currentChapterIndex + 1;
        if (nextChapterIndex <= total)
        {
            var nextChapUrl = docChapters.QuerySelector($"a.link-chap-{nextChapterIndex}").GetAttributeValue("href", null).Trim();
            nextChapId = ModelExtension.GetIDFromUrl(ModelType.Chapter, nextChapUrl).Trim();
        }
    }

    private void GetNameContentStoryOfChapter(HtmlDocument chapterDoc, out string chapterName, out string content, out Story story)
    {
        chapterName = chapterDoc.QuerySelector(@"body > div.container.body-container > div.content > div > h2").GetDirectInnerTextDecoded();
        content = chapterDoc.QuerySelector(@"div.chapter-c > div.chapter-c-content > div.box-chap:not(.hidden)").GetDirectInnerTextDecoded();
        var storyUrl = chapterDoc.QuerySelector(@"body > div.container.body-container > div.content > div > h1 > a").GetAttributeValue("href", null);
        var storyId = ModelExtension.GetIDFromUrl(ModelType.Story, storyUrl);
        story = GetStory(storyId);
    }

    // Extra functions for later uses
    // Extra functions for later uses
    // Extra functions for later uses

    private Category GetExactCategory(string id)
    {
        var doc = GetWebPageDocument(ModelExtension.GetUrlFromID(ModelType.Category, id));
        var name = doc.QuerySelector("body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.rank-header > div > div > div > div > p:nth - child(1) > a.act").GetDirectInnerTextDecoded();
        return new Category(name, id);
    }

    // Pages that structure like this:
    // https://truyen.tangthuvien.vn/tong-hop?ctg=1&limit=10000
    internal static class RankViewListFormat
    {
        // https://truyen.tangthuvien.vn/tong-hop?ctg=1&limit=10
        public static IEnumerable<Story> CrawlStoriesFromAPage(HtmlDocument doc)
        {
            var lisSelector = "#rank-view-list > div > ul > li";
            var aTagSelector = "div.book-mid-info > h4 > a";
            var imageSelector = "div.book-img-box > a > img";
            var authorNameSelector = @"div.book-mid-info > p.author > a.name";
            var liTags = doc.QuerySelectorAll(lisSelector);
            var stories = new List<Story>();
            foreach (var li in liTags)
            {
                var aTag = li.QuerySelector(aTagSelector);
                if (aTag == null)
                {
                    continue;
                }
                var image = li.QuerySelector(imageSelector);
                var url = aTag.GetAttributeValue("href", null) ?? throw new Exception();
                if (url.Contains("truyen.tangthuvien.vn") == false)
                {
                    continue;
                }
                var name = aTag.GetDirectInnerTextDecoded();
                var imageUrl = image.GetAttributeValue("src", null);
                var authorName = li.QuerySelector(authorNameSelector).GetDirectInnerTextDecoded();
                stories.Add(new Story(name, ModelExtension.GetIDFromUrl(ModelType.Story, url), imageUrl, authorName));
            }
            return stories;
        }

        public static bool IsLastPage(HtmlDocument doc)
        {
            var liAbleNextPageSelector = "body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.page-box.cf > div > ul > li:not(.disabled):last-child";
            return doc.QuerySelector(liAbleNextPageSelector) == null;
        }
    }

    [GeneratedRegex(@"ctg=\d+")]
    private static partial Regex GetCTG();
}
