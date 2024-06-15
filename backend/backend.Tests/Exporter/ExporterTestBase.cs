using Backend.Application.Queries;
using Backend.Domain.Contract;
using TruyenFullPlugin;

namespace Backend.Tests.Exporter;

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


public abstract class ExporterTestBase
{
    protected abstract IExporter GetExporter();

    [Fact]
    public void ExportFile_Call_ReturnsValidBytes()
    {
        // Arrange
        var exporter = GetExporter();
        var tf = new TruyenFullCommand();
        var exp = new ChapterQuery(tf);
        var lc = exp.GetChapterContents("dinh-menh-cho-ta-gap-nhau");
        var sd = tf.GetStoryDetail("dinh-menh-cho-ta-gap-nhau");

        // Act
        var bytes = exporter.ExportStory(sd, lc);

        // Assert
        Assert.NotEmpty(bytes);
    }
}
