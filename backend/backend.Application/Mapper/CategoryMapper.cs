using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper;

public static class CategoryMapper
{
    public static DisplayDTO ToDTO(this Category category)
    {
        return new DisplayDTO(category.Name, category.ID);
    }
}
