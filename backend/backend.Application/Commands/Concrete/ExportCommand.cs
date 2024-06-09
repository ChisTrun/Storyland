using backend.Application.Commands.Abstract;
using backend.Application.DTO;
using backend.Domain.Contract;
using backend.Domain.Entities;

namespace backend.Application.Commands.Concrete;

public class ExportCommand : IExportCommand
{
    private readonly IExporter _exporter;

    public ExportCommand(IExporter exporter)
    {
        _exporter = exporter;
    }

    public FileBytesDTO CreateFile(StoryDetail storyDetail, IEnumerable<ChapterContent> chapterContents)
    {
        var extension = _exporter.Extension;
        var bytes = _exporter.ExportStory(storyDetail, chapterContents);
        return new FileBytesDTO(bytes, extension);
    }
}
