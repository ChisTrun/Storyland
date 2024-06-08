namespace backend.Application.DTO;

public class FileBytesDTO(byte[] bytes, string extension)
{
    public byte[] Bytes { get; } = bytes;
    public string Extension { get; } = extension;
}
