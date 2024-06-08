using backend.Application.Commands.Abstract;
using backend.Application.Contract;
using backend.Application.DTO;
using backend.Application.Objects;
using backend.Application.Plugins.Contracts;

namespace backend.Application.Commands.Concrete;

public class ExportCommand : IExportCommand
{
    private readonly IExporter _exporter;

    public ExportCommand(IExporter exporter)
    {
        _exporter = exporter;
    }

    public FileBytesDTO CreateFile(StoryDetailDTO storyDetail, IEnumerable<ChapterContentDTO> chapterContents)
    {
        var extension = _exporter.Extension;
        var bytes = _exporter.ExportStory(storyDetail, chapterContents);
        return new FileBytesDTO(bytes, extension);
    }
}
