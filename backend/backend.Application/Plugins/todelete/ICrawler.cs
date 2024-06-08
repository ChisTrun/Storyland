using backend.Application.DTO;
using backend.Application.Mics;

namespace backend.Application.Plugins.Contracts;

public interface ICrawler : IPlugin
{
    public List<DisplayDTO> GetCategories();

    public List<StoryDTO> GetStoriesOfCategory(string categoryId);
    public PagedList<StoryDTO> GetStoriesOfCategory(string categoryId, int page, int limit);

    public List<StoryDTO> GetStoriesBySearchName(string storyName);
    public PagedList<StoryDTO> GetStoriesBySearchName(string storyName, int page, int limit);

    public List<StoryDTO> GetStoriesOfAuthor(string authorId);
    public PagedList<StoryDTO> GetStoriesOfAuthor(string authorId, int page, int limit);

    public List<ChapterDTO> GetChaptersOfStory(string storyId);
    public PagedList<ChapterDTO> GetChaptersOfStory(string storyId, int page, int limit);

    public ChapterContentDTO GetChapterContent(string storyId, int chapterIndex);

    public StoryDetailDTO GetStoryDetail(string storyId);

    public List<DisplayDTO> GetAuthorsBySearchName(string authorName);
    public PagedList<DisplayDTO> GetAuthorsBySearchName(string authorName, int page, int limit);

    public int GetChaptersCount(string storyId);
}
