using Backend.Application.DTO;
using Backend.Domain.Entities;

namespace Backend.Application.Mapper
{
    public static class StoryDetailMapper
    {
        public static StoryDetailDTO ToDTO(this StoryDetail storyDetail)
        {
            return new StoryDetailDTO(storyDetail.Identity, storyDetail.Name, storyDetail.ImageURL, storyDetail.Author.ToDTO(), storyDetail.Status, storyDetail.Categories.Select(x => x.ToDTO()), storyDetail.Description);
        }
    }
}
