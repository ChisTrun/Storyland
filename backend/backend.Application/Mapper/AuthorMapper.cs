using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper;

public static class AuthorMapper
{
    public static DisplayDTO ToDTO(this Author author)
    {
        return new DisplayDTO(author.Name, author.ID);
    }
}
