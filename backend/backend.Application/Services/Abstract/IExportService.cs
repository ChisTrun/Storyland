using backend.Application.DTO;
using backend.Domain.Mics;

namespace backend.Application.Services.Abstract;

public interface IExportService
{
    public List<PluginInfo> GetExportFormats();
    public FileBytesDTO CreateFile(string serverId, string fileTypeId, string storyId);
}
