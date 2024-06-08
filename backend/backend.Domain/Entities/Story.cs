using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public sealed class Story(string iD, string name, Author author, string imageURL, string status, string description, IEnumerable<Category> categories) : EntityBase(iD)
{
    public string Name { get; } = name;
    public Author Author { get; } = author;
    public string ImageURL { get; } = imageURL;
    public string Status { get; } = status;
    public string Description { get; } = description;
    public IEnumerable<Category> Categories { get; } = categories;
}
