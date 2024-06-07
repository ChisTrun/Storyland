using PluginBase.Contract;
using PluginBase.Models;
using System;
using TangThuVien;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace plugin.tests.Crawler;

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
    private static bool ValidRepresentative(Representative rep) => string.IsNullOrEmpty(rep.Id) == false && string.IsNullOrEmpty(rep.Name) == false;

    private void AssertPaging<T>(Func<int, int, PagingRepresentative<T>> getPage) where T : Representative
    {
        const int FIRST_PAGE = 1;
        const int LIMIT = 10;
        // Act
        var firstPageData = getPage(FIRST_PAGE, LIMIT);
        var lastPageCount = getPage(firstPageData.TotalPages, LIMIT).Data.Count();
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
        var isAllValidItem = list.All(ValidRepresentative);

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
        var isAllValidItem = stories.Data.All(ValidRepresentative);

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
    // + valid category ID
    // + valid paging
    // + invalid category ID
    // ===============================

}
