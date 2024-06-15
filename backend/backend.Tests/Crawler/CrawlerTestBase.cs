using Backend.Domain.Contract;
using Backend.Domain.Entities;
using Backend.Domain.Objects;
using Backend.Domain.Primitives;

namespace Backend.Tests.Crawler;

// ==========================================================================================
// Group15 document reference:
// https://docs.google.com/document/d/1PBs_CaLPvwPwL1NOHO7R7OHERQn3E2awAYwPehC2xCY/edit
// ==========================================================================================

/// <summary>
/// General interface test for every Crawler. 
/// Ref: https://stackoverflow.com/questions/9367673/can-i-implement-a-series-of-reusable-tests-to-test-an-interfaces-implementation
/// General test data (Theory)
/// Ref: https://stackoverflow.com/questions/55004351/xunit-abstract-test-class-with-virtual-test-methods-with-parameters
/// Skip test based on condition:
/// Ref: https://stackoverflow.com/questions/14840172/skipping-a-whole-test-class-in-xunit-net
/// Total 14 crawler function to test
/// </summary>


public abstract class CrawlerTestBase
{
    private static bool ValidIdentity(EntityBase rep) => string.IsNullOrEmpty(rep.Identity) == false;
    private static bool ValidChapterContent(ChapterContent chapterContent)
    {
        return string.IsNullOrEmpty(chapterContent.Content) == false
            && string.IsNullOrEmpty(chapterContent.Name) == false
            && ValidIdentity(chapterContent) == true;
    }
    private static bool ValidStoryDetail(StoryDetail storyDetail)
    {
        return storyDetail.Author != null
            && storyDetail.Categories != null
            && string.IsNullOrEmpty(storyDetail.Status) == false
            && string.IsNullOrEmpty(storyDetail.Description) == false
            && ValidIdentity(storyDetail) == true;
    }

    private void AssertPaging<T>(Func<int, int, PagedList<T>> getPage) where T : class
    {
        // Arrange
        var firstPage = 1;
        var limit = 10;

        // Act
        var firstPageData = getPage(firstPage, limit);
        var lastPageCount = getPage(firstPageData.TotalPages, limit).Data.Count();
        var emptyFirstPage = !firstPageData.Data.Any();

        // Assert
        Assert.True(!emptyFirstPage || lastPageCount == 0);
        Assert.True(emptyFirstPage || lastPageCount > 0);
    }

    protected static async Task<bool> CheckIfServerAlive(string url)
    {
        using HttpClient client = new();
        var checkingResponse = await client.GetAsync(url);
        if (!checkingResponse.IsSuccessStatusCode)
        {
            return false;
        }
        return true;
    }

    protected abstract ICrawler GetInstance();

    // ===============================
    // GetCategories
    // =============================== 

    [Fact]
    public void GetCategories_Call_ReturnsAllValidItem()
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var list = crawler.GetCategories();
        var isAllValidItem = list.All(ValidIdentity);

        // Assert
        Assert.True(isAllValidItem);
    }

    // ===============================
    // GetStoriesOfCategory
    // + valid category ID
    // + valid paging
    // + invalid category ID
    // ===============================

    public virtual void GetStoriesOfCategory_ValidCategoryId_ReturnsStories(string categoryId)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var stories = crawler.GetStoriesOfCategory(categoryId, 1, 10);
        var isAllValidItem = stories.Data.All(ValidIdentity);

        // Assert
        Assert.True(isAllValidItem);
    }

    public virtual void GetStoriesOfCategory_ValidCategoryId_ValidPaging(string categoryId)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        // Assert
        AssertPaging((page, limit) => crawler.GetStoriesOfCategory(categoryId, page, limit));
    }

    public virtual void GetStoriesOfCategory_InvalidCategoryId_ThrowsException(string categoryId)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetStoriesOfCategory(categoryId, 1, 10);

        // Assert
        Assert.Throws<Exception>(testCode);
    }

    // ===============================
    // GetStoriesBySearchName
    // + valid story name
    // + valid paging
    // + invalid story name
    // ===============================

    public virtual void GetStoriesBySearchName_ValidStoryName_ReturnsStories(string storyName)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var stories = crawler.GetStoriesBySearchName(storyName, 1, 10);
        var isAllValidItem = stories.Data.All(ValidIdentity);

        // Assert
        Assert.True(isAllValidItem);
    }

    public virtual void GetStoriesBySearchName_ValidStoryName_ValidPaging(string storyName)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        // Assert
        AssertPaging((page, limit) => crawler.GetStoriesBySearchName(storyName, page, limit));
    }

    public virtual void GetStoriesBySearchName_InvalidStoryName_ThrowsException(string storyName)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetStoriesBySearchName(storyName, 1, 10);

        // Assert
        Assert.Throws<Exception>(testCode);
    }

    // ===============================
    // GetStoriesOfAuthor
    // + valid author ID
    // + valid paging
    // + invalid author ID
    // ===============================

    public virtual void GetStoriesOfAuthor_ValidAuthorId_ReturnsStories(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var stories = crawler.GetStoriesOfAuthor(id, 1, 10);
        var isAllValidItem = stories.Data.All(ValidIdentity);

        // Assert
        Assert.True(isAllValidItem);
    }

    public virtual void GetStoriesOfAuthor_ValidAuthorId_ValidPaging(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        // Assert
        AssertPaging((page, limit) => crawler.GetStoriesOfAuthor(id, page, limit));
    }

    public virtual void GetStoriesOfAuthor_InvalidAuthorId_ThrowsException(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetStoriesOfAuthor(id, 1, 10);

        // Assert
        Assert.Throws<Exception>(testCode);
    }

    // ===============================
    // GetChaptersOfStory
    // + valid story ID
    // + valid paging
    // + invalid story ID
    // ===============================

    public virtual void GetChaptersOfStory_ValidStoryId_ReturnChapters(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var chapters = crawler.GetChaptersOfStory(id, 1, 10);
        var isAllValidItem = chapters.Data.All(ValidIdentity);

        // Assert
        Assert.True(isAllValidItem);
    }

    public virtual void GetChaptersOfStory_ValidPaging_ReturnChapters(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        // Assert
        AssertPaging((page, limit) => crawler.GetChaptersOfStory(id, page, limit));
    }

    public virtual void GetChaptersOfStory_InvalidStoryId_ThrowsException(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetChaptersOfStory(id, 1, 10);

        // Assert
        Assert.Throws<Exception>(testCode);
    }

    // ===============================
    // GetChapterContent
    // + valid story ID, chapter index
    // + invalid story ID
    // + invalid chapter index
    // ===============================

    public virtual void GetChapterContent_ValidStoryId_ReturnChapterContent(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var chapter = crawler.GetChapterContent(id, 0);
        var isAllValidItem = ValidChapterContent(chapter);

        // Assert
        Assert.True(isAllValidItem);
    }

    public virtual void GetChapterContent_InvalidChapterIndex_ThrowsException(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetChapterContent(id, -1);

        // Assert
        Assert.Throws<Exception>(testCode);
    }

    public virtual void GetChapterContent_InvalidStoryId_ThrowsException(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetChapterContent(id, 0);

        // Assert
        Assert.Throws<Exception>(testCode);
    }

    // ===============================
    // GetStoryDetail
    // + valid story ID
    // + invalid story ID
    // ===============================

    public virtual void GetStoryDetail_ValidStoryId_ReturnStoryDetail(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var story = crawler.GetStoryDetail(id);
        var isAllValidItem = ValidStoryDetail(story);

        // Assert
        Assert.True(isAllValidItem);
    }

    public virtual void GetStoryDetail_InvalidStoryId_ThrowsException(string id)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetStoryDetail(id);

        // Assert
        Assert.Throws<Exception>(testCode);
    }

    // ===============================
    // GetAuthorsBySearchName
    // + valid name
    // + empty name
    // ===============================

    public virtual void GetAuthorsBySearchName_ValidName_ReturnsAuthors(string name)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var authors = crawler.GetAuthorsBySearchName(name, 1, 10);
        var isAllValidItem = authors.Data.All(ValidIdentity);

        // Assert
        Assert.True(isAllValidItem);
    }

    public virtual void GetAuthorsBySearchName_EmptyName_ThrowsException(string name)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetAuthorsBySearchName(name, 1, 10);

        // Assert
        Assert.Throws<Exception>(testCode);
    }

    // ===============================
    // GetChaptersCount
    // + valid story id
    // + invalid story id
    // ===============================

    public virtual void GetChapterCount_ValidStoryId_ReturnChaptersCount(string storyId)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        var count = crawler.GetChaptersCount(storyId);
        var isAllValidItem = count >= 0;

        // Assert
        Assert.True(isAllValidItem);
    }

    public virtual void GetChapterCount_InValidStoryId_ThrowsException(string storyId)
    {
        // Arrange
        var crawler = GetInstance();

        // Act
        void testCode() => crawler.GetChaptersCount(storyId);

        // Assert
        Assert.Throws<Exception>(testCode);
    }
}
