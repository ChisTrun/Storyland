namespace PluginBase.Models
{
    public class PagingRepresentative<T>(int page, int limit, int totalPages, IEnumerable<T> data) where T : Representative
    {
        public int Page { get; set; } = page;
        public int Limit { get; set; } = limit;
        public int TotalPages { get; set; } = totalPages;
        public IEnumerable<T> Data { get; set; } = data;
    }
}
