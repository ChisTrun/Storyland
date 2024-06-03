namespace PluginBase.Models
{
    public class Story : Representative
    {
        public string ImageUrl { get; set; }
        public string? AuthorName { get; }

        /// <param name="name" example="Kiếm Lai"></param>
        /// <param name="id" exmaple="kiem-lai"></param>
        /// <param name="imageUrl" example="https://www.nae.vn/ttv/ttv/public/images/story/af6eb3e654cc2282cad5a1d7b5f8ba149ed358cb521d8bf63b9a3c8aec88f03f.jpg"></param>
        public Story(string name, string id, string? imageUrl, string? authorName) : base(name, id)
        {
            ImageUrl = imageUrl ?? "https://birkhauser.de/product-not-found.png";
            AuthorName = authorName;
        }
    }
}
