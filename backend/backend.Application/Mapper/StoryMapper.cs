using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper;

public static class StoryMapper
{
    public static StoryDTO ToDTO(this Story story)
    {
        return new StoryDTO(story.Identity, story.Name, story.ImageURL, story.AuthorName ?? "");
    }
}
