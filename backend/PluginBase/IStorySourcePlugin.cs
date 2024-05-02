using PluginBase.Models;
using System.Xml;

namespace PluginBase;

public interface IStorySourcePlugin
{
    public string Name { get; }
    public string Description { get; }

    /// <summary>
    /// Lay tat ca the loai truyen cua trang
    /// </summary>
    /// <returns>Mot danh sach cac the loai</returns>
    public IEnumerable<Categories> GetCategories();

    /// <summary>
    /// Lay thong tin cac truyen thuoc mot the loai hoac mot danh sach nao do
    /// </summary>
    /// <param name="url">Url cua mot the loai</param>
    /// <returns>Danh sach cac truyen thuoc the loai do</returns>
    public IEnumerable<StoryInfo> GetStoryInfoOfCategory(string categoryName);

    /// <summary>
    ///Tim kiem thong tin cac truyen dua tren tu khoa
    /// </summary>
    /// <param name="searchWord">Tu khoa tim kiem</param>
    /// <returns>Danh sach cac truyen la ket qua tim kiem</returns>
    public IEnumerable<StoryInfo> GetStoriesFromSearchingName(string searchWord);

    /// <summary>
    /// Tim thong tin truyen dua tren 'Chinh Xac' ten tac gia
    /// </summary>
    /// <param name="searchWord">Ten tac gia</param>
    /// <returns>Danh sach cac truyen la ket qua tim kiem</returns>
    public IEnumerable<StoryInfo> GetStoriesFromSearchingExactAuthor(string searchWord);

    /// <summary>
    /// Lay thong tin cac chuong cua mot truyen
    /// </summary>
    /// <param name="Url">Link toi cac chuong cua mot truyen</param>
    /// <returns>Danh sach cac chuong cua truyen do</returns>
    public List<ChapterInfo> GetChaptersOfStory(string sourceURL);

    /// <summary>
    /// Lay noi dung cua mot chuong cua mot truyen
    /// </summary>
    /// <param name="sourceURL">Link toi trang noi dung cua chuong</param>
    /// <returns>Noi dung cua trang truyen</returns>
    public ChapterContent GetChapterContent(string sourceURL);
}
