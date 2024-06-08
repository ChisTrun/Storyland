using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public sealed class Author(string id, string name) : EntityBase(id)
{
    public string Name { get; } = name;
}
