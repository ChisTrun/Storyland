using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public sealed class Story(string iD, string name, string? imageURL = null, Author? author = null) : EntityBase(iD)
{
    public string Name { get; } = name;
    public string ImageURL { get; } = imageURL ?? "https://birkhauser.de/product-not-found.png";
    public Author? Author { get; } = author;
}
