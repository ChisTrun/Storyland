﻿using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public sealed class Category(string name, string id) : IDEntityBase(id)
{
    public string Name { get; } = name;
}
