namespace backend.Application.DTO;

public class SourceStoriesDTO(string iD, List<StoryDTO> stories)
{
    public string ID { get; } = iD;
    public List<StoryDTO> Stories { get; } = stories;
}
