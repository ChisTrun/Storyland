using Backend.Application.DLLScanner.Contract;
using Backend.Application.DTO;
using Backend.Application.Mapper;
using Backend.Application.Utils;
using Backend.Domain.Contract;

namespace Backend.Application.Queries;

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
                var stories = crawler.GetStoriesBySearchName(storyId).ToDTOList(x => x.ToDTO())
                .OrderBy(x => x.Id)
                .ToList();
                return stories;
            }
            catch (Exception)
            {
                return new List<StoryDTO>();
            }
        });
        var storiesDtoPriority = Algorithm.PriorityMergeLists(storiesList, s => s.Id, (listList, min, resList) =>
        {
            var minByIdList = listList.Select(l => l.FirstOrDefault(x => x.Id.Equals(min.Id)));
            var maxNumberOfChapter = minByIdList.Max(x => x?.NumberOfChapter ?? 0);
            var foundMax = false;
            foreach (var list in listList)
            {
                var first = list.FirstOrDefault(x => x.Id == min.Id);
                if (first != null)
                {
                    if (first.NumberOfChapter < maxNumberOfChapter || foundMax == true)
                    {
                        list.Remove(first);
                    }
                    else
                    {
                        foundMax = true;
                        resList.Add(first);
                        list.Remove(first);
                    }
                }
            }
        });
        var filteredStoriesDtoPriority = storiesDtoPriority.Where(x =>
        {
            return
            (minChapNum == -1 || x.NumberOfChapter >= minChapNum) &&
            (maxChapNum == -1 || x.NumberOfChapter <= maxChapNum);
        }).ToList();
        return filteredStoriesDtoPriority;
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
