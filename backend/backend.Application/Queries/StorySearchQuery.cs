using backend.Application.DLLScanner.Contract;
using backend.Application.DTO;
using backend.Application.Mapper;
using backend.Application.Utils;
using backend.Domain.Contract;

namespace backend.Application.Queries;

public class StorySearchQuery
{
    private readonly IScanner<ICrawler> _scanner;

    public StorySearchQuery(IScanner<ICrawler> scanner)
    {
        _scanner = scanner;
    }

    public List<StoryDTO> SearchAllStoriesWithPriority(string storyId, IEnumerable<string> ids, int minChapNum, int maxChapNum)
    {
        var storiesList = Algorithm.CrawlMultipleSources(_scanner, ids, (crawler) =>
        {
            try
            {
                var stories = crawler.GetStoriesBySearchName(storyId).Where(x =>
                {
                    return
                    (minChapNum == -1 || x.NumberOfChapter >= minChapNum) &&
                    (maxChapNum == -1 || x.NumberOfChapter <= maxChapNum);
                }).ToDTOList(x => x.ToDTO())
                .OrderBy(x => x.Id)
                .ToList();
                return stories;
            }
            catch (Exception)
            {
                return new List<StoryDTO>();
            }
        });
        var storiesDtoPriority = Algorithm.PriorityMergeLists(storiesList, s => s.Id);
        return storiesDtoPriority;
    }

    public List<StoryDTO> OfCategoryWithPriority(string categoryId, IEnumerable<string> ids)
    {
        var storiesList = Algorithm.CrawlMultipleSources(_scanner, ids, (crawler) =>
        {
            try
            {
                var stories = crawler.GetStoriesOfCategory(categoryId).ToDTOList(x => x.ToDTO())
                .OrderBy(x => x.Id)
                .ToList();
                return stories;
            }
            catch (Exception)
            {
                return new List<StoryDTO>();
            }
        });
        var storiesDtoPriority = Algorithm.PriorityMergeLists(storiesList, s => s.Id);
        return storiesDtoPriority;
    }
}
