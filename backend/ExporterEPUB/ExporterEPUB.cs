using backend.Domain.Contract;
using backend.Domain.Entities;
using backend.Domain.Objects;

namespace ExporterEPUB;

public class EPUBExport : IExporter
{
    public string Extension => "epub";

    public string Name => "EPUB";

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

    public byte[] ExportStory(StoryDetail story, IEnumerable<ChapterContent> chapteres)
    {
        throw new NotImplementedException();
    }
}
