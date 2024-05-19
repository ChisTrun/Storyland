namespace PluginBase.Models
{
    public class PagingRepresentative(int page, int limit, int totalPages, IEnumerable<Representative> data)
    {
        public int Page { get; set; } = page;
        public int Limit { get; set; } = limit;
        public int TotalPages { get; set; } = totalPages;
        public IEnumerable<Representative> Data { get; set; } = data;
    }
}
