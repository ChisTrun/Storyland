using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper
{
    public static class StoryDetailMapper
    {
        public static StoryDetailDTO ToDTO(this StoryDetail storyDetail)
        {
            return new StoryDetailDTO(storyDetail.ID, storyDetail.Name, storyDetail.ImageURL, storyDetail.Author.ToDTO(), storyDetail.Status, storyDetail.Categories.Select(x => x.ToDTO()), storyDetail.Description);
        }
    }
}
