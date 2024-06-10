using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public sealed class Story(string name, string iD, string? imageURL = null, string? authorName = null) : EntityBase(iD)
{
    public string Name { get; } = name;
    public string ImageURL { get; } = imageURL ?? "https://birkhauser.de/product-not-found.png";
    public string? AuthorName { get; } = authorName;
}
