using Backend.Application.DTO;

namespace Backend.Application.Services.Abstract;

public interface IExportService
{
    public FileBytesDTO CreateFile(string serverId, string fileTypeId, string storyId);
}
