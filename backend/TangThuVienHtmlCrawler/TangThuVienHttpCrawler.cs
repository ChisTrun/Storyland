﻿using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using PluginBase.Contract;
using PluginBase.Models;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace TangThuVienPlugin;

public class TangThuVienHttpCrawler : HtmlCrawlerBase
{
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

    public override string Name => "Tàng Thư Viện";

    public override IEnumerable<Category> GetCategories()
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
    public override IEnumerable<Story> GetStoriesOfCategory(string categoryName)
    {
        var stories = new List<Story>();
        var categories = GetCategories();
        var categoryUrl = categories.FirstOrDefault((cate) => cate.Name == categoryName)?.Url ?? throw new Exception();
        var document = GetWebPageDocument($"{categoryUrl}&{AllLimitParam}");    // Can be devided into pages and fetch async => speed
        var storiesSelector = "#rank-view-list > div > ul > li > div.book-mid-info > h4 > a";
        var storiesATags = document.QuerySelectorAll(storiesSelector);
        foreach (var storyATag in storiesATags)
        {
            var url = storyATag.GetAttributeValue("href", null) ?? throw new Exception();
            var name = WebUtility.HtmlDecode(storyATag.GetDirectInnerText());
            stories.Add(new Story(name, url));
        }
        return stories;
    }

    // https://truyen.tangthuvien.vn/ket-qua-tim-kiem?term=dinh&page=2
    public override IEnumerable<Story> GetStoriesBySearchName(string storyName)
    {
        var stories = new List<Story>();
        var baseSearchUrl = $"{DomainKetQuaTimKiem}?term={WebUtility.UrlEncode(storyName)}";
        var liAbleNextPageSelector = "body > div.rank-box.box-center.cf > div.main-content-wrap.fl > div.page-box.cf > div > ul > li:not(.disabled):last-child";
        var page = 1;
        IEnumerable<Story> GetStoriesBySearchNameByPage(int page, HtmlDocument document)
        {
            var stories = new List<Story>();
            var storiesSelector = "#rank-view-list > div > ul > li > div.book-mid-info > h4 > a";
            var storiesATags = document.QuerySelectorAll(storiesSelector);
            foreach (var storyATag in storiesATags)
            {
                var url = storyATag.GetAttributeValue("href", null) ?? throw new Exception();
                var name = WebUtility.HtmlDecode(storyATag.GetDirectInnerText());
                stories.Add(new Story(name, url));
            }
            return stories;
        }
        bool LastPage(HtmlDocument document) => document.QuerySelector(liAbleNextPageSelector) == null;
        var tasks = new List<Task<IEnumerable<Story>>>();
        while (true)
        {
            var document = GetWebPageDocument($"{baseSearchUrl}&page={page}");
            tasks.Add(Task.Run(() => GetStoriesBySearchNameByPage(page, document)));
            if (LastPage(document) == true)
            {
                break;
            }
            page += 1;
        }
        var resultTask = Task.WhenAll(tasks);
        resultTask.Wait();
        var storiesArray = resultTask.Result;
        foreach (var storiesItem in storiesArray)
        {
            stories.AddRange(storiesItem);
        }
        return stories;
    }

    // No need Headless
    public override IEnumerable<Story> GetStoriesOfAuthor(string authorName)
    {
        var storyInfos = new List<Story>();
        //var author = GetAuthorInfoFromExactName(authorName);
        //if (author != null)
        //{
        //    var authorUrl = author.Url;
        //    var puppeteerTask = PuppeteerService.PerformHeadlessBrowser(async (browser, page) =>
        //    {
        //        await page.GoToAsync(authorUrl);
        //        var aTagsSelector = "#rank-view-list>div.book-img-text>ul>li>div.book-mid-info>h4>a";
        //        var aTags = await page.QuerySelectorAllAsync(aTagsSelector);
        //        foreach (var aTag in aTags)
        //        {
        //            var url = await aTag.GetPropertyStringValueAsync("href");
        //            var name = await aTag.GetInnerTextAsync();
        //            storyInfos.Add(new Story(name, url));
        //        }
        //    });
        //    puppeteerTask.Wait();
        //}
        return storyInfos;
    }

    // No need headless
    // https://truyen.tangthuvien.vn/doc-truyen/page/38020?page=0&limit=100000&web=1
    public override List<Chapter> GetChaptersOfStory(string storyURL)
    {
        var chapterInfos = new List<Chapter>();
        //var puppeteerTask = PuppeteerService.PerformHeadlessBrowser(async (browser, page) =>
        //{
        //    await page.GoToAsync(storyURL);
        //    var catalogAnchorSelector = "#j-bookCatalogPage";
        //    var changeTabCommand = await page.GetOnclickScript(catalogAnchorSelector);
        //    await page.EvaluateExpressionAsync(changeTabCommand);
        //    var chapterListSelector = "#max-volume>ul.cf>li>a";
        //    var nextPageSelector = """#max-volume>div.list-chapter>nav.nav-pagination>ul.pagination>li>a[aria-label="Next"]""";
        //    while (true)
        //    {
        //        var aTags = await page.QuerySelectorAllAsync(chapterListSelector);
        //        if (aTags.Length == 0)
        //        {
        //            break;
        //        }
        //        foreach (var aTag in aTags)
        //        {
        //            var fontTag = await aTag.QuerySelectorAsync("font>font");
        //            var url = await aTag.GetPropertyStringValueAsync("href");
        //            var name = await fontTag.GetInnerTextAsync();
        //            chapterInfos.Add(new Chapter(name, url));
        //        }
        //        var nextPageCommand = await page.GetOnclickScript(nextPageSelector);
        //        if (string.IsNullOrEmpty(nextPageCommand))
        //        {
        //            break;
        //        }
        //        // the command call jQuery.ajax which is asynchronous
        //        // can use MutationObserver but too complex => wait for active page changes
        //        await page.EvaluateExpressionAsync(nextPageCommand);
        //        var currentPageAnchorSelector = "#max-volume>div.list-chapter>nav.nav-pagination>ul.pagination>li.active>a";
        //        var currentPage = await page.EvaluateExpressionAsync<string>(
        //            $"(document.querySelector(\"{currentPageAnchorSelector}\")).innerText");
        //        var waitExpr = $"\"{currentPage}\" !== (document.querySelector(\"{currentPageAnchorSelector}\")).innerText";
        //        await page.WaitForExpressionAsync(waitExpr);
        //    }
        //});
        //puppeteerTask.Wait();
        return chapterInfos;
    }

    // No need headless
    // https://truyen.tangthuvien.vn/doc-truyen/page/38020?page=1&limit=1&web=1
    public override ChapterContent GetChapterContent(string chapterURL)
    {
        ChapterContent? chapterContent = null;
        //var puppeteerTask = PuppeteerService.PerformHeadlessBrowser(async (browser, page) =>
        //{
        //    await page.GoToAsync(chapterURL);
        //    var contentDiv = await page.QuerySelectorAsync("div.chapter-c>div.chapter-c-content>div.box-chap:not(.hidden)");
        //    var content = await contentDiv.GetInnerTextAsync();
        //    Func<string, Task<string>> GetUrlOnClick = async (selector) =>
        //    {
        //        // can't use normal GetPropertyAsync on event type
        //        var pageNavigateCommand = await page.GetOnclickScript(selector);
        //        await page.EvaluateExpressionAsync<string>(pageNavigateCommand);
        //        // could be either navigate or not because of the function
        //        // https://stackoverflow.com/questions/64406833/how-to-check-if-a-puppeteer-page-is-currently-in-navigation-state
        //        await page.WaitForSelectorAsync("html");
        //        var url = await page.EvaluateExpressionAsync<string>("window.location.href");
        //        if (page.Url != chapterURL)
        //        {
        //            await page.GoBackAsync();
        //        }
        //        return url;
        //    };
        //    var nextChapSelector = "#next_chap";
        //    var prevChapSelector = "#prev_chap";
        //    var prevChapUrl = await GetUrlOnClick.Invoke(prevChapSelector);
        //    var nextChapUrl = await GetUrlOnClick.Invoke(nextChapSelector);
        //    chapterContent = new ChapterContent(content, nextChapUrl, prevChapUrl);
        //});
        //puppeteerTask.Wait();
        return chapterContent ?? throw new Exception();
    }

    // Hàm thêm vào
    // Hàm thêm vào
    // Hàm thêm vào

    private Author? GetAuthorInfoFromExactName(string name)
    {
        var authors = GetAuthorsBySearchName(name);
        foreach (var author in authors)
        {
            if (author.Name == name)
            {
                return author;
            }
        }
        return null;
    }

    private Story? GetStoryFromExactName(string name)
    {
        var authors = GetStoriesBySearchName(name);
        foreach (var author in authors)
        {
            if (author.Name == name)
            {
                return author;
            }
        }
        return null;
    }

    // note: search engine của trang này không được ổn định
    public IEnumerable<Author> GetAuthorsBySearchName(string name)
    {
        var storyInfos = new List<Author>();
        //var domainTimKiemWithKey = $"{DomainTimKiem}?term={WebUtility.UrlEncode(name)}";
        //Task.Run(async () =>
        //{
        //    using var client = new HttpClient();
        //    try
        //    {
        //        var response = await client.GetAsync(DomainTimKiem);
        //        response.EnsureSuccessStatusCode();
        //        string jsonData = await response.Content.ReadAsStringAsync();
        //        var jsonTypeList = new[] { new { id = 0, name = "", url = "", type = "", story_type = 0 } };
        //        var resArray = JsonConvert.DeserializeAnonymousType(jsonData, jsonTypeList) ?? throw new Exception("Null response");
        //        foreach (var res in resArray)
        //        {
        //            // story_type == 1 leads to ngontinh.tangthuvien
        //            if (res.type == "author")
        //            {
        //                storyInfos.Add(new Author(res.name, $"{DomainTacGia}?author={res.id}"));
        //            }
        //        }
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        Console.WriteLine($"HTTP request error: {ex.Message}");
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"{ex.Message}");
        //        throw;
        //    }
        //}).Wait();
        return storyInfos;
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
