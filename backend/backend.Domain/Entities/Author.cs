using Backend.Domain.Primitives;

namespace Backend.Domain.Entities;

public sealed class Author(string name, string id) : IDEntityBase(id)
{
    public string Name { get; } = name;
}
