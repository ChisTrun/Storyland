using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json;
using PluginBase.Contract;
using PluginBase.Models;
using System.Net;
using System.Web;

namespace TangThuVienHttp;

public class TangThuVienHttpCrawler : ICrawler
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

    private readonly string _domain = "https://truyen.tangthuvien.vn";
    private string DomainTongHop => $"{_domain}/tong-hop";
    private string DomainTimKiem => $"{_domain}/tim-kiem";
    private string DomainKetQuaTimKiem => $"{_domain}/ket-qua-tim-kiem";
    private string DomainDocTruyen => $"{_domain}/doc-truyen";
    private string DomainTacGia => $"{_domain}/tac-gia";
    private string AllLimitParam => "limit=100000";

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
            var name = WebUtility.HtmlDecode(categoryATag.GetDirectInnerText());
            categories.Add(new Category(name, url));
        }
        return categories;
    }

    // https://truyen.tangthuvien.vn/tong-hop?ctg=1&limit=100000
    public IEnumerable<Story> GetStoriesOfCategory(string categoryName)
    {
        var categories = GetCategories();
        var categoryUrl = categories.FirstOrDefault((cate) => cate.Name == categoryName)?.Url ?? throw new Exception();

        // Devided into pages fetch => increase speed
        var baseUrl = $"{categoryUrl}&limit=10";
        var createUrlFromPage = new CreateURLFromPage((page) => $"{baseUrl}&page={page}");
        IEnumerable<Story> stories = GetRepresentativesFromATagsFromAllPages(RankViewListFormat.CrawlStoriesFromAPage, createUrlFromPage.Invoke, RankViewListFormat.IsLastPage);
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

    // https://truyen.tangthuvien.vn/tac-gia?author=27&page=1
    public IEnumerable<Story> GetStoriesOfAuthor(string authorName)
    {
        var author = GetAuthorInfoFromExactName(authorName) ?? throw new Exception();
        var createUrlFromPage = new CreateURLFromPage((page) => $"{author.Url}&page={page}");
        IEnumerable<Story> stories = GetRepresentativesFromATagsFromAllPages(RankViewListFormat.CrawlStoriesFromAPage, createUrlFromPage.Invoke, RankViewListFormat.IsLastPage);
        return stories;
    }

    // https://truyen.tangthuvien.vn/doc-truyen/page/38020?page=0&limit=100000&web=1
    public List<Chapter> GetChaptersOfStory(string storyName)
    {
        var story = GetStoryFromExactName(storyName);
        if (story != null)
        {
            var id = GetWebPageDocument(story.Url).QuerySelector("#story_id_hidden").GetAttributeValue("value", null) ?? throw new Exception();
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
                    var name = WebUtility.HtmlDecode(fontTag.GetDirectInnerText());
                    chapters.Add(new Chapter(name, url, count));
                    count++;
                }
            }
            return chapters;
        }
        throw new Exception();
    }

    // https://truyen.tangthuvien.vn/doc-truyen/page/38020?page=1&limit=1&web=1
    public ChapterContent GetChapterContent(string storyName, int chapterIndex)
    {
        var chapters = GetChaptersOfStory(storyName);
        //var chapterIndex = GetChapterIndexFromExactNameFromStory(storyName, chapterName);
        var chapter = chapters[chapterIndex];
        var document = GetWebPageDocument(chapter.Url);
        var contentSelector = "div.chapter-c > div.chapter-c-content > div.box-chap:not(.hidden)";
        var content = WebUtility.HtmlDecode(document.QuerySelector(contentSelector).GetDirectInnerText());
        var prevChapUrl = chapters[Math.Max(chapterIndex - 1, 0)].Url;
        var nextChapUrl = chapters[Math.Min(chapterIndex + 1, chapters.Count - 1)].Url;
        return new ChapterContent(content, nextChapUrl, prevChapUrl);
    }

    private delegate string CreateURLFromPage(int page);

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

    // Extra functions for later uses
    // Extra functions for later uses
    // Extra functions for later uses

    private Author? GetAuthorInfoFromExactName(string name)
    {
        var authors = GetAuthorsBySearchName(name);
        var author = authors.FirstOrDefault((author) => author.Name == name) ?? throw new Exception();
        return author;
    }

    private Story? GetStoryFromExactName(string name)
    {
        var stories = GetStoriesBySearchName(name);
        var story = stories.FirstOrDefault((story) => story.Name == name) ?? throw new Exception();
        return story;
    }

    // note: &nbsp and ' ' may look the same but they aren't
    private int GetChapterIndexFromExactNameFromStory(string storyName, string name)
    {
        var chapters = GetChaptersOfStory(storyName);
        var chapterIndex = chapters.FindIndex((story) => story.Name == name);
        return chapterIndex;
    }

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
                        authors.Add(new Author(res.name, $"{DomainTacGia}?author={res.id}"));
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

    public IEnumerable<Story> GetStoryInfoOfCategoryByPage(string categoryUrl, int offset0, int limit)
    {
        var storyInfos = new List<Story>();
        //var currentPage = 1;
        //var aTagsSelector = "#rank-view-list > div > ul > li > div.book-mid-info > h4 > a";
        //var liAbleNextPageSelector = "body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.page-box.cf > div > ul > li:not(.disabled):last-child";
        //static string GetCategoryPageUrlFunc(string categoryUrl, int currentPage) => $"{categoryUrl}&page={currentPage}";
        //static async Task<bool> CheckAbleToGoNextPageFunc(string? liAbleNextPageSelector, IPage page) => (await page.QuerySelectorAsync(liAbleNextPageSelector)) != null;
        //var puppeteerTask = PuppeteerService.PerformHeadlessBrowser(async (browser, page) =>
        //{
        //    var perPage = -1;
        //    await page.GoToAsync(GetCategoryPageUrlFunc(categoryUrl, currentPage));
        //    var aTags = await page.QuerySelectorAllAsync(aTagsSelector);
        //    if (await CheckAbleToGoNextPageFunc(liAbleNextPageSelector, page) == false)
        //    {
        //        var count = 0;
        //        var len = aTags.Length;
        //        for (var i = 0; i < len; ++i)
        //        {
        //            if (count == limit)
        //            {
        //                break;
        //            }
        //            var url = await aTags[i].GetPropertyStringValueAsync("href");
        //            var name = await aTags[i].GetInnerTextAsync();
        //            storyInfos.Add(new Story(name, url));
        //            count++;
        //        }
        //    }
        //    else
        //    {
        //        perPage = aTags.Length;
        //        var lowPage = (offset0 / perPage) + 1;
        //        var highPage = ((offset0 + limit) / perPage) + 1;
        //        currentPage = lowPage;
        //        while (currentPage <= highPage)
        //        {
        //            await page.GoToAsync(GetCategoryPageUrlFunc(categoryUrl, currentPage));
        //            aTags = await page.QuerySelectorAllAsync(aTagsSelector);
        //            var start = currentPage == lowPage ? offset0 % perPage : 0;
        //            var end = currentPage == highPage ? (offset0 + limit - 1) % perPage : perPage - 1;
        //            for (var i = start; i <= end; ++i)
        //            {
        //                var url = await aTags[i].GetPropertyStringValueAsync("href");
        //                var name = await aTags[i].GetInnerTextAsync();
        //                storyInfos.Add(new Story(name, url));
        //            }
        //            if (await CheckAbleToGoNextPageFunc(liAbleNextPageSelector, page) == false)
        //            {
        //                break;
        //            }
        //            currentPage += 1;
        //        }
        //    }
        //});
        //puppeteerTask.Wait();
        return storyInfos;
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
                var name = WebUtility.HtmlDecode(aTag.GetDirectInnerText());
                var imageUrl = image.GetAttributeValue("src", null);
                stories.Add(new Story(name, url, imageUrl));
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
