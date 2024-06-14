using backend.Application.DLLScanner.Contract;
using backend.Application.DTO;
using backend.Application.Mapper;
using backend.Domain.Contract;

namespace backend.Application.Queries;

public class StorySearchQuery
{
    private readonly IScanner<ICrawler> _scanner;

    public StorySearchQuery(IScanner<ICrawler> scanner)
    {
        _scanner = scanner;
    }

    public List<StoryDTO> SearchAllStroy(string storyId, IEnumerable<string> ids, int minChapNum, int maxChapNum)
    {
        var tasks = new List<Task<List<StoryDTO>>>();
        foreach (var id in ids)
        {
            tasks.Add(Task.Run(() =>
            {
                var plugin = _scanner.GetAllPlugins()[id].Plugin;
                var stories = plugin.GetStoriesBySearchName(storyId).Where(x =>
                {
                    return
                    (minChapNum == -1 || x.NumberOfChapter >= minChapNum) &&
                    (maxChapNum == -1 || x.NumberOfChapter <= maxChapNum);
                }).ToDTOList(x => x.ToDTO())
                .OrderBy(x => x.Id)
                .ToList();
                return stories;
            }));
        }
        var storiesList = Task.WhenAll(tasks).Result.ToList();
        var storiesDto = new List<StoryDTO>();
        while (true)
        {
            var minStory = storiesList.Select(x => x.FirstOrDefault()).Where(x => x != null).MinBy(x => x!.Id);
            if (minStory == null)
            {
                break;
            }
            storiesList.ForEach(x =>
            {
                var first = x.FirstOrDefault(s => s.Id == minStory.Id);
                if (first != null)
                {
                    x.Remove(first);
                }
            });
            storiesDto.Add(minStory);
        }
        return storiesDto;
    }
}
