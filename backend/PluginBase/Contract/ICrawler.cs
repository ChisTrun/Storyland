using PluginBase.Models;

namespace PluginBase.Contract;

public interface ICrawler
{
    public string Name { get; }
    public string Description { get; }

    /// <summary>
    /// Lay tat ca the loai truyen cua trang
    /// </summary>
    /// <returns>Mot danh sach cac the loai</returns>
    public IEnumerable<Category> GetCategories();

    /// <summary>
    /// Lay tat ca truyen thuoc mot the loai hoac mot danh sach nao do
    /// </summary>
    /// <param name="categoryName">Ten the loai</param>
    /// <returns>Danh sach cac truyen thuoc the loai do</returns>
    public IEnumerable<Story> GetStoriesOfCategory(string categoryName);

    /// <summary>
    /// Tim kiem truyen theo ten
    /// </summary>
    /// <param name="storyName">Ten truyen</param>
    /// <returns>Danh sach cac truyen la ket qua tim kiem</returns>
    public IEnumerable<Story> GetStoriesBySearchName(string storyName);

    /// <summary>
    /// Tim thong tin truyen dua tren 'Chinh Xac' ten tac gia
    /// </summary>
    /// <param name="authorName">Ten tac gia</param>
    /// <returns>Danh sach cac truyen la ket qua tim kiem</returns>
    public IEnumerable<Story> GetStoriesOfAuthor(string authorName);

    /// <summary>
    /// Lay thong tin cac chuong cua mot truyen
    /// </summary>
    /// <param name="storyName">Ten truyen</param>
    /// <returns>Danh sach cac chuong cua truyen do</returns>
    public List<Chapter> GetChaptersOfStory(string storyName);

    /// <summary>
    /// Lay noi dung cua mot chuong cua mot truyen
    /// </summary>
    /// <param name="chapterName">Ten mot chuong cua truyen</param>
    /// <returns>Noi dung cua trang truyen</returns>
    public ChapterContent GetChapterContent(string storyName, int chapterIndex);
}
