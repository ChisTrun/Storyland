using Backend.Domain.Contract;
using Backend.Domain.Entities;

namespace ExporterEPUB;

public class EPUBExport : IExporter
{
    public string Extension => "epub";

    public string Name => "EPUB";

    public byte[] ExportStory(StoryDetail story, IEnumerable<ChapterContent> chapters)
    {
        byte[] byteStream;
        using (var serve = new ExporterEPUBServe(story, chapters.ToList()))
        {
            byteStream = serve.ExportEpub();
        }
        return byteStream;
    }
}
