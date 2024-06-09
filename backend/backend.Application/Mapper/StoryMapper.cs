using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper;

public static class StoryMapper
{
    public static StoryDTO ToDTO(this Story story)
    {
        return new StoryDTO(story.ID, story.Name, story.ImageURL, story.Author?.Name ?? "");
    }
}
