using PluginBase.Contract;
using PluginBase.Models;

namespace ExporterEPUB;

public class EPUBExport : IExporter
{
    public byte[] ExportStory(StoryDetail story, List<ChapterContent> chapters)
    {
        byte[] byteStream;
        using (var serve = new ExporterEPUBServe(story, chapters))
        {
            byteStream = serve.ExportEpub();
        }
        return byteStream;
    }

    public byte[] ExportChapter(StoryDetail story, ChapterContent chapters)
    {
        byte[] byteStream;
        using (var serve = new ExporterEPUBServe(story, new List<ChapterContent>() { chapters }))
        {
            byteStream = serve.ExportEpub();
        }
        return byteStream;
    }
}
