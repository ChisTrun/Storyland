using Backend.Domain.Objects;

namespace Backend.Application.Mapper;

public static class PagedListMapper
{
    public static PagedList<TDto> ToDTOList<TEntity, TDto>(this PagedList<TEntity> pagedList, Func<TEntity, TDto> cast) where TEntity : class where TDto : class
    {
        return new PagedList<TDto>(pagedList.Page, pagedList.Limit, pagedList.TotalPages, pagedList.Data.Select(x => cast(x)));
    }
}

public static class ListMapper
{
    public static List<TDto> ToDTOList<TEntity, TDto>(this IEnumerable<TEntity> pagedList, Func<TEntity, TDto> cast) where TEntity : class where TDto : class
    {
        return pagedList.Select(x => cast(x)).ToList();
    }
}
