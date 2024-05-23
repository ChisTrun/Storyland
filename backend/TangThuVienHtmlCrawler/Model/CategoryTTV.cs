using PluginBase.Models;

namespace TangThuVienHtmlCrawler.Model
{
    public class CategoryTTV
    {
        private readonly Category _category;

        public CategoryTTV(Category category)
        {
            _category = category;
        }
        public string Name => _category.Name;
    }
}
