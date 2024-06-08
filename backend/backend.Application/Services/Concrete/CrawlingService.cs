using backend.Application.DTO;
using backend.Application.Mics;
using backend.Application.Objects;
using backend.Application.Plugins.Abstract;
using backend.Application.Queries.Abstract;
using backend.Application.Queries.Concrete;
using backend.Application.Services.Abstract;
using System.Collections.Generic;

namespace backend.Application.Services.Concrete;

public class CrawlingService : ICrawlingService
{
    private readonly IPluginProvider _pluginProvider;

    public CrawlingService(IPluginProvider pluginProvider)
    {
        _pluginProvider = pluginProvider;
    }

    // ====

    public PagedList<DisplayDTO> GetAuthorBySearchName(string serverId, string authorName, int page, int limit)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var authors = crawler.GetAuthorsBySearchName(authorName, page, limit);
        return authors;
    }

    public List<DisplayDTO> GetAuthorsBySearchName(string serverId, string authorName)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var authors = crawler.GetAuthorsBySearchName(authorName);
        return authors;
    }

    // ====

    public List<DisplayDTO> GetCategories(string serverId)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var categories = crawler.GetCategories();
        return categories;
    }

    // ====

    public List<StoryDTO> GetStoriesOfAuthor(string serverId, string authorId)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetStoriesOfAuthor(authorId);
        return stories;
    }

    public PagedList<StoryDTO> GetStoriesOfAuthor(string serverId, string authorId, int page, int limit)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetStoriesOfAuthor(authorId, page, limit);
        return stories;
    }

    public List<StoryDTO> GetStoriesOfCategory(string serverId, string categoryId)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetStoriesOfCategory(categoryId);
        return stories;
    }

    public PagedList<StoryDTO> GetStoriesOfCategory(string serverId, string categoryId, int page, int limit)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetStoriesOfCategory(categoryId, page, limit);
        return stories;
    }

    public List<StoryDTO> GetStoriesBySearchName(string serverId, string searchName)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetStoriesBySearchName(searchName);
        return stories;
    }

    public PagedList<StoryDTO> GetStoriesBySearchName(string serverId, string searchName, int page, int limit)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetStoriesBySearchName(searchName, page, limit);
        return stories;
    }

    // ====

    public StoryDetailDTO GetStoryDetail(string serverId, string storyId)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var storyDetail = crawler.GetStoryDetail(storyId);
        return storyDetail;
    }

    public List<ChapterDTO> GetChaptersOfStory(string serverId, string storyId)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetChaptersOfStory(storyId);
        return stories;
    }

    public PagedList<ChapterDTO> GetChaptersOfStory(string serverId, string storyId, int page, int limit)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetChaptersOfStory(storyId, page, limit);
        return stories;
    }

    public ChapterContentDTO GetChapterContent(string serverId, string storyId, int chapterIndex)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var stories = crawler.GetChapterContent(storyId, chapterIndex);
        return stories;
    }

    // ====

    public List<PluginInfo> GetServers()
    {
        return _pluginProvider.GetCrawlers();
    }
}
