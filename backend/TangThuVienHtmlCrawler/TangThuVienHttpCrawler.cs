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

    public string Description => throw new NotImplementedException();

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
        var document = GetWebPageDocument($"{categoryUrl}&{AllLimitParam}");    // Can be devided into pages and fetch async => speed
        var storiesSelector = "#rank-view-list > div > ul > li > div.book-mid-info > h4 > a";
        var storiesATags = document.QuerySelectorAll(storiesSelector);
        var stories = GetRepresentativesFromATags(_createStory, storiesATags);
        return stories;
    }

    // https://truyen.tangthuvien.vn/ket-qua-tim-kiem?term=dinh&page=2
    public IEnumerable<Story> GetStoriesBySearchName(string storyName)
    {
        var baseSearchUrl = $"{DomainKetQuaTimKiem}?term={WebUtility.UrlEncode(storyName)}";
        var liAbleNextPageSelector = "body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.page-box.cf > div > ul > li:not(.disabled):last-child";
        var representativesATagSelector = "#rank-view-list > div > ul > li > div.book-mid-info > h4 > a";
        var createUrlFromPage = new CreateURLFromPage((page) => $"{baseSearchUrl}&page={page}");
        IEnumerable<Story> stories = GetRepresentativesFromATagsWithMultiplePages(_createStory, createUrlFromPage, liAbleNextPageSelector, representativesATagSelector);
        return stories;
    }

    // https://truyen.tangthuvien.vn/tac-gia?author=27&page=1
    public IEnumerable<Story> GetStoriesOfAuthor(string authorName)
    {
        var author = GetAuthorInfoFromExactName(authorName);
        if (author != null)
        {
            var createUrlFromPage = new CreateURLFromPage((page) => $"{author.Url}&page={page}");
            var authorsSelector = "#rank-view-list > div.book-img-text > ul > li > div.book-mid-info > h4 > a";
            var liAbleNextPageSelector = "body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.page-box.cf > div > ul > li:not(.disabled):last-child";
            IEnumerable<Story> stories = GetRepresentativesFromATagsWithMultiplePages(_createStory, createUrlFromPage, liAbleNextPageSelector, authorsSelector);
            return stories;
        }
        throw new Exception();
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

    // private functions
    // private functions
    // private functions


    private delegate T CreateRepresentative<T>(string name, string url) where T : Representative;
    private CreateRepresentative<Story> _createStory = new CreateRepresentative<Story>((name, url) => new Story(name, url));
    private CreateRepresentative<Author> _createAuthor = new CreateRepresentative<Author>((name, url) => new Author(name, url));
    private delegate string CreateURLFromPage(int page);

    private static IEnumerable<T> GetRepresentativesFromATagsWithMultiplePages<T>(CreateRepresentative<T> createRepresentative, CreateURLFromPage createURLFromPage, string liAbleNextPageSelector, string representativesATagSelector) where T : Representative
    {
        var page = 1;
        IEnumerable<T> GetRepresentativesFromAPage(int page, HtmlDocument document)
        {
            var storiesATags = document.QuerySelectorAll(representativesATagSelector);
            return GetRepresentativesFromATags(createRepresentative, storiesATags);
        }
        bool IsLastPage(HtmlDocument document) => document.QuerySelector(liAbleNextPageSelector) == null;
        var tasks = new List<Task<IEnumerable<T>>>();
        while (true)
        {
            var accessUrl = createURLFromPage.Invoke(page);
            var document = GetWebPageDocument(accessUrl);
            tasks.Add(Task.Run(() => GetRepresentativesFromAPage(page, document)));
            if (IsLastPage(document) == true)
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

    private static IEnumerable<T> GetRepresentativesFromATags<T>(CreateRepresentative<T> createRepresentative, IList<HtmlNode> storiesATags) where T : Representative
    {
        var representatives = new List<T>();
        foreach (var storyATag in storiesATags)
        {
            var url = storyATag.GetAttributeValue("href", null) ?? throw new Exception();
            var name = WebUtility.HtmlDecode(storyATag.GetDirectInnerText());
            representatives.Add(createRepresentative.Invoke(name, url));
        }
        return representatives;
    }

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
}
