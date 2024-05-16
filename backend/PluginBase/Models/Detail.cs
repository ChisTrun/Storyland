namespace PluginBase.Models
{
    public class StoryDetail : Story
    {
        public Author Author { get; }
        public string Status { get; }
        public Category[] Categories { get; }
        public string Description { get; }

        public StoryDetail(string name, string url, string? imageUrl, Author author, string status, Category[] categories, string description) : base(name, url, imageUrl)
        {
            Author = author;
            Status = status;
            Categories = categories;
            Description = description;
        }

        public StoryDetail(Story story, Author author, string status, Category[] categories, string description) : this(story.Name, story.Url, story.ImageUrl, author, status, categories, description)
        {
        }
    }
}
