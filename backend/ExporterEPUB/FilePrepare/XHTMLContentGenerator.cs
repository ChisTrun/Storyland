using ExporterEPUB.Helpers;
using ExporterEPUB.Model;
using ExporterEPUB.XHTMLBuilder;
using ExporterEPUB.XHTMLBuilder.Content;
using PluginBase.Models;

namespace ExporterEPUB.FilePrepare;

public class XHTMLContentGenerator
{
    public class ChapterDocument
    {
        public XHTMLDocument Document { get; }
        public string Title { get; }

        public ChapterDocument(XHTMLDocument chapterDoc, string title)
        {
            Document = chapterDoc;
            Title = title;
        }
    }

    public class ContentStructure
    {
        public ContentStructure(XHTMLDocument cover, XHTMLDocument origin, XHTMLDocument intro, List<ChapterDocument> chapters)
        {
            Cover = cover;
            Origin = origin;
            Intro = intro;
            Chapters = chapters;
        }
        public XHTMLDocument Cover { get; }
        public XHTMLDocument Origin { get; }
        public XHTMLDocument Intro { get; }
        public List<ChapterDocument> Chapters { get; }
    }

    private readonly string _storyName;
    private readonly string _authorName;
    private readonly string _storyDescription;
    private readonly string _storyCategories;
    private readonly string _storyStatus;
    private readonly List<ChapterContent> _chapters;
    private readonly string _imagePath;

    public static readonly string CSS_PATH = $"../Styles/style.css";

    public XHTMLContentGenerator(string storyName, string authorName, string storyDescription, string storyCategories, string storyStatus, List<ChapterContent> chapters, string imagePath)
    {
        _storyName = storyName;
        _authorName = authorName;
        _storyDescription = storyDescription;
        _storyCategories = storyCategories;
        _storyStatus = storyStatus;
        _chapters = chapters;
        _imagePath = Path.Join("..", "Images", Path.GetFileName(imagePath)).InverseSlash();
    }

    private LayoutXHTML GenerateLayout() => new LayoutXHTML(_storyName, CSS_PATH);

    private XHTMLDocument GenerateCover()
    {
        var layout = GenerateLayout();
        var intro = new CoverXHTML(_imagePath);
        return intro.SetContent(layout).GetXHTML();
    }

    private XHTMLDocument GenerateIntro()
    {
        var layout = GenerateLayout();
        var intro = new IntroXHTML(_storyName, _authorName, _storyDescription, _storyCategories, _storyStatus, _imagePath);
        return intro.SetContent(layout).GetXHTML();
    }

    private XHTMLDocument GenerateOrigin()
    {
        var layout = GenerateLayout();
        var intro = new OriginXHTML();
        return intro.SetContent(layout).GetXHTML();
    }

    private XHTMLDocument GenerateChapter(ChapterContent chapter)
    {
        var layout = GenerateLayout();
        var intro = new ChapterXHTML(chapter.ChapterName, chapter.Content);
        return intro.SetContent(layout).GetXHTML();
    }

    public ContentStructure CreateDocumentStructure()
    {
        var coverDoc = GenerateCover();
        var originDoc = GenerateOrigin();
        var introDoc = GenerateIntro();
        var chapterDocs = new List<ChapterDocument>();
        foreach (var chapter in _chapters)
        {
            chapterDocs.Add(new ChapterDocument(GenerateChapter(chapter), chapter.ChapterName));
        }
        return new ContentStructure(coverDoc, originDoc, introDoc, chapterDocs);
    }
}
