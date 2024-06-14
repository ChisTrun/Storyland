using backend.Application.DLLScanner.Contract;
using backend.Application.DTO;
using backend.Application.Mapper;
using backend.Application.Utils;
using backend.Domain.Contract;

namespace backend.Application.Queries;

public class CategoryQuery
{
    private readonly IScanner<ICrawler> _scanner;

    public CategoryQuery(IScanner<ICrawler> scanner)
    {
        _scanner = scanner;
    }

    public List<DisplayDTO> AllCategoriesWithPriority(IEnumerable<string> ids)
    {
        var categoriesList = Algorithm.CrawlMultipleSources(_scanner, ids, (crawler) =>
        {
            return crawler.GetCategories().ToDTOList(x => x.ToDTO()).OrderBy(x => Algorithm.ConvertToUnsign(x.Id)).ToList();
        });
        var categoryDtoPriority = Algorithm.PriorityMergeLists(categoriesList, x => Algorithm.ConvertToUnsign(x.Id));
        return categoryDtoPriority;
    }
}
