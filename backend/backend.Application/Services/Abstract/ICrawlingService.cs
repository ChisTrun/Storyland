using backend.Application.DTO;
using backend.Domain.Objects;

namespace backend.Application.Services.Abstract;

public interface ICrawlingService
{
    List<DisplayDTO> GetAuthorsBySearchName(string serverId, string authorName);
    PagedList<DisplayDTO> GetAuthorBySearchName(string serverId, string authorName, int page, int limit);
    List<DisplayDTO> GetCategories(string serverId);
    List<StoryDTO> GetStoriesOfAuthor(string serverId, string authorId);
    PagedList<StoryDTO> GetStoriesOfAuthor(string serverId, string authorId, int page, int limit);
    List<StoryDTO> GetStoriesOfCategory(string serverId, string categoryId);
    PagedList<StoryDTO> GetStoriesOfCategory(string serverId, string categoryId, int page, int limit);
    List<StoryDTO> GetStoriesBySearchName(string serverId, string searchName);
    PagedList<StoryDTO> GetStoriesBySearchName(string serverId, string searchName, int page, int limit);
    ChapterContentDTO GetChapterContent(string serverId, string storyId, int chapterIndex);
    PagedList<ChapterDTO> GetChaptersOfStory(string serverId, string storyId, int page, int limit);
    List<ChapterDTO> GetChaptersOfStory(string serverId, string storyId);
    StoryDetailDTO GetStoryDetail(string serverId, string storyId);
    int GetChaptersCount(string serverId, string storyId);
    PagedList<StoryDTO> GetStoriesBySearchNameWithFilter(string serverId, string searchName, int minChapNum, int maxChapNum, int page, int limit);
    List<StoryDTO> GetStoriesWithPriority(IEnumerable<string> idsWithPriority, string storyId, int minChapNum, int maxChapNum);
    List<StoryDTO> GetStoriesOfCategoryWithPriority(IEnumerable<string> idsWithPriority, string categoryId);
    List<DisplayDTO> GetCategoriesWithPriority(IEnumerable<string> idsWithPriority);
}
