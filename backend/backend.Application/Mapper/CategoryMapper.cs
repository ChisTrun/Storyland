using Backend.Application.DTO;
using Backend.Domain.Entities;

namespace Backend.Application.Mapper;

public static class CategoryMapper
{
    public static DisplayDTO ToDTO(this Category category)
    {
        return new DisplayDTO(category.Name, category.Identity);
    }
}
