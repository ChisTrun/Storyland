using backend.Domain.Contract;
using plugin.tests.Crawler;
using TangThuVien;

namespace PluginBase.Tests.Crawler.ConcreteTest;

public class TangThuVienCrawlerTest : CrawlerTestBase
{
    protected override ICrawler GetInstance()
    {
        return new TangThuVienCrawler();
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
}
