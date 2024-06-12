using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public class StoryDetail(string id, string name, string imageURL, Author author, string status, IEnumerable<Category> categories, string description) : IDEntityBase(id)
{
    public StoryDetail(Story story, Author author, string status, IEnumerable<Category> categories, string description) : this(story.Identity, story.Name, story.ImageURL, author, status, categories, description)
    {
    }

    public string Name { get; } = name;
    public string ImageURL { get; } = imageURL ?? "https://birkhauser.de/product-not-found.png";
    public Author Author { get; } = author;
    public string Status { get; } = status;
    public string Description { get; } = description;
    public IEnumerable<Category> Categories { get; } = categories;

    public Story ToStory() => new(Identity, Name, ImageURL, Author.Name);
}
