using backend.Application.DTO;
using backend.Application.Objects;

namespace backend.Application.Services.Abstract;

public interface IExportService
{
    public List<PluginInfo> GetExportFormats();
    public FileBytesDTO CreateFile(string serverId, string fileTypeId, string storyId);
}
