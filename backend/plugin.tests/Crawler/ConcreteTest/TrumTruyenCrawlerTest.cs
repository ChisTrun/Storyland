using PluginBase.Contract;
using TrumTruyenHtmlCrawler;

namespace plugin.tests.Crawler.ConcreteTest;

public class TrumTruyenCrawlerTest : CrawlerTestBase
{
    protected override ICrawler GetInstance()
    {
        return new TrumTruyenCrawler();
    }

    [Theory]
    [InlineData("tien-hiep")]
    public override void GetStoriesOfCategory_ValidCategoryId_ReturnsStories(string categoryId)
    {
        base.GetStoriesOfCategory_ValidCategoryId_ReturnsStories(categoryId);
    }

    [Theory]
    [InlineData("tien-hiep")]
    public override void GetStoriesOfCategory_ValidCategoryId_ValidPaging(string categoryId)
    {
        base.GetStoriesOfCategory_ValidCategoryId_ValidPaging(categoryId);
    }

    [Theory]
    [InlineData("tien-hiepppp")]
    public override void GetStoriesOfCategory_InvalidCategoryId_ThrowsException(string categoryId)
    {
        base.GetStoriesOfCategory_InvalidCategoryId_ThrowsException(categoryId);
    }

    [Theory]
    [InlineData("kiem-lai")]
    public override void GetStoriesBySearchName_ValidStoryName_ReturnsStories(string storyName)
    {
        base.GetStoriesBySearchName_ValidStoryName_ReturnsStories(storyName);
    }

    [Theory]
    [InlineData("kiem-lai")]
    public override void GetStoriesBySearchName_ValidStoryName_ValidPaging(string storyName)
    {
        base.GetStoriesBySearchName_ValidStoryName_ValidPaging(storyName);
    }

    [Theory]
    [InlineData("kiem-laiiii")]
    public override void GetStoriesBySearchName_InvalidStoryName_ThrowsException(string storyName)
    {
        base.GetStoriesBySearchName_InvalidStoryName_ThrowsException(storyName);
    }

    [Theory]
    [InlineData("phong-hoa-hi-chu-hau")]
    public override void GetStoriesOfAuthor_ValidAuthorId_ReturnsStories(string id)
    {
        base.GetStoriesOfAuthor_ValidAuthorId_ReturnsStories(id);
    }

    [Theory]
    [InlineData("phong-hoa-hi-chu-hau")]
    public override void GetStoriesOfAuthor_ValidAuthorId_ValidPaging(string id)
    {
        base.GetStoriesOfAuthor_ValidAuthorId_ValidPaging(id);
    }

    [Theory]
    [InlineData("phong-hoa-hi-chu-hau")]
    public override void GetStoriesOfAuthor_InvalidAuthorId_ThrowsException(string id)
    {
        base.GetStoriesOfAuthor_InvalidAuthorId_ThrowsException(id);
    }

    [Theory]
    [InlineData("kiem-lai")]
    public override void GetChaptersOfStory_ValidStoryId_ReturnChapters(string id)
    {
        base.GetChaptersOfStory_ValidStoryId_ReturnChapters(id);
    }

    [Theory]
    [InlineData("kiem-lai")]
    public override void GetChaptersOfStory_ValidPaging_ReturnChapters(string id)
    {
        base.GetChaptersOfStory_ValidPaging_ReturnChapters(id);
    }

    [Theory]
    [InlineData("kiem-laiiii")]
    public override void GetChaptersOfStory_InvalidStoryId_ThrowsException(string id)
    {
        base.GetChaptersOfStory_InvalidStoryId_ThrowsException(id);
    }

    [Theory]
    [InlineData("kiem-lai")]
    public override void GetChapterContent_ValidStoryId_ReturnChapterContent(string id)
    {
        base.GetChapterContent_ValidStoryId_ReturnChapterContent(id);
    }

    [Theory]
    [InlineData("kiem-lai")]
    public override void GetChapterContent_InvalidChapterIndex_ThrowsException(string id)
    {
        base.GetChapterContent_InvalidChapterIndex_ThrowsException(id);
    }

    [Theory]
    [InlineData("kiem-laiiii")]
    public override void GetChapterContent_InvalidStoryId_ThrowsException(string id)
    {
        base.GetChapterContent_InvalidStoryId_ThrowsException(id);
    }

    [Theory]
    [InlineData("kiem-lai")]
    public override void GetStoryDetail_ValidStoryId_ReturnStoryDetail(string id)
    {
        base.GetStoryDetail_ValidStoryId_ReturnStoryDetail(id);
    }

    [Theory]
    [InlineData("kiem-laiiii")]
    public override void GetStoryDetail_InvalidStoryId_ThrowsException(string id)
    {
        base.GetStoryDetail_InvalidStoryId_ThrowsException(id);
    }

    [Theory]
    [InlineData("phong hoa hi")]
    public override void GetAuthorsBySearchName_ValidName_ReturnsAuthors(string name)
    {
        base.GetAuthorsBySearchName_ValidName_ReturnsAuthors(name);
    }

    [Theory]
    [InlineData("")]
    public override void GetAuthorsBySearchName_EmptyName_ThrowsException(string name)
    {
        base.GetAuthorsBySearchName_EmptyName_ThrowsException(name);
    }

    [Theory]
    [InlineData("kiem-lai")]
    public override void GetChapterCount_ValidStoryId_ReturnChaptersCount(string storyId)
    {
        base.GetChapterCount_ValidStoryId_ReturnChaptersCount(storyId);
    }

    [Theory]
    [InlineData("kiem-laiiii")]
    public override void GetChapterCount_InValidStoryId_ThrowsException(string storyId)
    {
        base.GetChapterCount_InValidStoryId_ThrowsException(storyId);
    }
}
