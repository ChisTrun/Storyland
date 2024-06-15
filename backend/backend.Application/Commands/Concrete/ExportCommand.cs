using Backend.Application.Commands.Abstract;
using Backend.Application.DTO;
using Backend.Domain.Contract;
using Backend.Domain.Entities;

namespace Backend.Application.Commands.Concrete;

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
