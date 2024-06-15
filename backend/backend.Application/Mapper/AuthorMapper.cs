using Backend.Application.DTO;
using Backend.Domain.Entities;

namespace Backend.Application.Mapper;

public static class AuthorMapper
{
    public static DisplayDTO ToDTO(this Author author)
    {
        return new DisplayDTO(author.Name, author.Identity);
    }
}
