using PluginBase.Models;

namespace TruyenFullPlugin.Models;

public class StoryTF
{
    public StoryTF(Story story, string authorName)
    {
        Story = story;
        AuthorName = authorName;
    }

    public Story Story { get; set; }
    public string AuthorName { get; set; }
}
