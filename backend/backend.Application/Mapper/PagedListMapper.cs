using backend.Domain.Entities;
using backend.Domain.Objects;

namespace backend.Application.Mapper;

public static class PagedListMapper
{
    public static PagedList<TDto> ToDTOList<TEntity, TDto>(this PagedList<TEntity> pagedList, Func<TEntity, TDto> Cast) where TEntity : class where TDto : class
    {
        return new PagedList<TDto>(pagedList.Page, pagedList.Limit, pagedList.TotalPages, pagedList.Data.Select(x => Cast(x)));
    }
}

public static class ListMapper
{
    public static List<TDto> ToDTOList<TEntity, TDto>(this List<TEntity> pagedList, Func<TEntity, TDto> Cast) where TEntity : class where TDto : class
    {
        return pagedList.Select(x => Cast(x)).ToList();
    }
}
