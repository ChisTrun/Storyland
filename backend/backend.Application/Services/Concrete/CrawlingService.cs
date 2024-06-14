using backend.Application.DTO;
using backend.Application.Mapper;
using backend.Application.Queries;
using backend.Application.Services.Abstract;
using backend.Domain.Objects;

namespace backend.Application.Services.Concrete;

public class CrawlingService : ICrawlingService
{
    private readonly IPluginsScannerService _pluginsScannerService;

    public CrawlingService(IPluginsScannerService pluginsScannerService)
    {
        _pluginsScannerService = pluginsScannerService;
    }

    // ====

    public PagedList<DisplayDTO> GetAuthorBySearchName(string serverId, string authorName, int page, int limit)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var authors = crawler.GetAuthorsBySearchName(authorName, page, limit).ToDTOList(a => a.ToDTO());
        return authors;
    }

    public List<DisplayDTO> GetAuthorsBySearchName(string serverId, string authorName)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var authors = crawler.GetAuthorsBySearchName(authorName).ToDTOList(x => x.ToDTO());
        return authors;
    }

    // ====

    public List<DisplayDTO> GetCategories(string serverId)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var categories = crawler.GetCategories().ToDTOList(x => x.ToDTO());
        return categories;
    }

    // ====

    public List<StoryDTO> GetStoriesOfAuthor(string serverId, string authorId)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetStoriesOfAuthor(authorId).ToDTOList(x => x.ToDTO());
        return stories;
    }

    public PagedList<StoryDTO> GetStoriesOfAuthor(string serverId, string authorId, int page, int limit)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetStoriesOfAuthor(authorId, page, limit).ToDTOList(x => x.ToDTO());
        return stories;
    }

    public List<StoryDTO> GetStoriesOfCategory(string serverId, string categoryId)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetStoriesOfCategory(categoryId).ToDTOList(x => x.ToDTO());
        return stories;
    }

    public PagedList<StoryDTO> GetStoriesOfCategory(string serverId, string categoryId, int page, int limit)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetStoriesOfCategory(categoryId, page, limit).ToDTOList(x => x.ToDTO());
        return stories;
    }

    public List<StoryDTO> GetStoriesBySearchName(string serverId, string searchName)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetStoriesBySearchName(searchName).ToDTOList(x => x.ToDTO());
        return stories;
    }

    public PagedList<StoryDTO> GetStoriesBySearchName(string serverId, string searchName, int page, int limit)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetStoriesBySearchName(searchName, page, limit).ToDTOList(x => x.ToDTO());
        return stories;
    }

    public PagedList<StoryDTO> GetStoriesBySearchNameWithFilter(string serverId, string searchName, int minChapNum, int maxChapNum, int page, int limit)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetStoriesBySearchNameWithFilter(searchName, minChapNum, maxChapNum, page, limit).ToDTOList(x => x.ToDTO());
        return stories;
    }

    // ====

    public StoryDetailDTO GetStoryDetail(string serverId, string storyId)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var storyDetail = crawler.GetStoryDetail(storyId).ToDTO();
        return storyDetail;
    }

    public List<ChapterDTO> GetChaptersOfStory(string serverId, string storyId)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetChaptersOfStory(storyId).ToDTOList(x => x.ToDTO());
        return stories;
    }

    public PagedList<ChapterDTO> GetChaptersOfStory(string serverId, string storyId, int page, int limit)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetChaptersOfStory(storyId, page, limit).ToDTOList(x => x.ToDTO());
        return stories;
    }

    public int GetChaptersCount(string serverId, string storyId)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetChaptersCount(storyId);
        return stories;
    }

    public ChapterContentDTO GetChapterContent(string serverId, string storyId, int chapterIndex)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var stories = crawler.GetChapterContent(storyId, chapterIndex).ToDTO();
        return stories;
    }

    // ====

    public List<StoryDTO> GetStoriesWithPriority(IEnumerable<string> idsWithPriority, string storyId, int minChapNum, int maxChapNum)
    {
        var query = new StorySearchQuery(_pluginsScannerService.GetCrawlerScanner());
        var stories = query.SearchAllStoriesWithPriority(storyId, idsWithPriority, minChapNum, maxChapNum);
        return stories;
    }

    public List<StoryDTO> GetStoriesOfCategoryWithPriority(IEnumerable<string> idsWithPriority, string categoryId)
    {
        var query = new StorySearchQuery(_pluginsScannerService.GetCrawlerScanner());
        var stories = query.OfCategoryWithPriority(categoryId, idsWithPriority);
        return stories;
    }

    public List<DisplayDTO> GetCategoriesWithPriority(IEnumerable<string> idsWithPriority)
    {
        var query = new CategoryQuery(_pluginsScannerService.GetCrawlerScanner());
        var categories = query.AllCategoriesWithPriority(idsWithPriority);
        return categories;
    }
}
