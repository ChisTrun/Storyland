using Backend.Application.DTO;
using Backend.Domain.Entities;

namespace Backend.Application.Mapper;

public static class StoryMapper
{
    public static StoryDTO ToDTO(this Story story)
    {
        return new StoryDTO(story.Identity, story.Name, story.ImageURL, story.AuthorName ?? "", story.NumberOfChapter);
    }
}
