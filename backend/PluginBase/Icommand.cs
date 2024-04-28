using PluginBase.Models;

namespace PluginBase;

public interface Icommand
{
    public string Name { get; }
    public string Description { get; }

    /// <summary>
    /// lay tat ca the loai cua trang web
    /// </summary>
    /// <returns>danh sach the loai</returns>
    public IEnumerable<Categories> GetCategories();

    /// <summary>
    /// lay thong tin cac truyen thuoc mot the loai hoac danh sach nao do
    /// </summary>
    /// <param name="url">nhan vao link cua the loai hoac danh sach</param>
    /// <returns>danh sach cac truyen thuoc the loai do</returns>
    public IEnumerable<StoryInfo> GetStoryInfos(string sourceURL);
}
