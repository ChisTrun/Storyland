using backend.Application.DTO;

namespace backend.Application.Services.Abstract;

public interface IExportService
{
    public FileBytesDTO CreateFile(string serverId, string fileTypeId, string storyId);
}
