using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public class StoryDetail(string id, string name, string imageURL, Author author, string status, string description, IEnumerable<Category> categories) : EntityBase(id)
{
    public StoryDetail(Story story, string status, string description, IEnumerable<Category> categories) : this(story.ID, story.Name, story.ImageURL, story.Author!, status, description, categories)
    {
        if (story.Author == null)
        {
            throw new ArgumentNullException("");
        }
    }

    public string Name { get; } = name;
    public string ImageURL { get; } = imageURL ?? "https://birkhauser.de/product-not-found.png";
    public Author Author { get; } = author;
    public string Status { get; } = status;
    public string Description { get; } = description;
    public IEnumerable<Category> Categories { get; } = categories;

    public Story ToStory() => new(ID, Name, ImageURL, Author);
}
