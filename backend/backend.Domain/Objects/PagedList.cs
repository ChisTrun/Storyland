namespace backend.Domain.Objects;

public class PagedList<T>(int page, int limit, int totalPages, IEnumerable<T> data)
{
    public int Page { get; set; } = page;
    public int Limit { get; set; } = limit;
    public int TotalPages { get; set; } = totalPages;
    public IEnumerable<T> Data { get; set; } = data;
}
