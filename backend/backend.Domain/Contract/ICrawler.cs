using backend.Domain.Entities;
using backend.Domain.Objects;

namespace backend.Domain.Contract;

public interface ICrawler : IPlugin
{
    public string Address { get; }

    public List<Category> GetCategories();

    public List<Story> GetStoriesOfCategory(string categoryId);
    public PagedList<Story> GetStoriesOfCategory(string categoryId, int page, int limit);

    public List<Story> GetStoriesBySearchName(string storyName);
    public PagedList<Story> GetStoriesBySearchName(string storyName, int page, int limit);

    public List<Story> GetStoriesOfAuthor(string authorId);
    public PagedList<Story> GetStoriesOfAuthor(string authorId, int page, int limit);

    public List<Chapter> GetChaptersOfStory(string storyId);
    public PagedList<Chapter> GetChaptersOfStory(string storyId, int page, int limit);
    public int GetChaptersCount(string storyId);

    public ChapterContent GetChapterContent(string storyId, int chapterIndex);

    public StoryDetail GetStoryDetail(string storyId);

    public List<Author> GetAuthorsBySearchName(string authorName);
    public PagedList<Author> GetAuthorsBySearchName(string authorName, int page, int limit);
}
