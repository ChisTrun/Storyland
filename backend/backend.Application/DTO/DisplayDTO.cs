namespace Backend.Application.DTO;

/// <summary>
/// Display a general object with Name and Id. Includes: Category, Author, ...
/// </summary>
/// 
public class DisplayDTO(string name, string id)
{
    public string Name { get; } = name;
    public string Id { get; } = id;
}
