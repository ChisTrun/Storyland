using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public sealed class Author(string name, string id) : EntityBase(id)
{
    public string Name { get; } = name;
}
