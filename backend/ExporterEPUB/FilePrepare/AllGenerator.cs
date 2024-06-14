using backend.Domain.Entities;
using ExporterEPUB.Helpers;
using ExporterEPUB.Model;
using ExporterEPUB.XHTMLBuilder;

namespace ExporterEPUB.FilePrepare;

public class AllGenerator
{
    private readonly StoryDetail _story;
    private readonly List<ChapterContent> _chapterContents;
    private readonly string _imagePath;
    private readonly FolderStructure _structure;

    public AllGenerator(StoryDetail story, List<ChapterContent> chapterContents, string imagePath, FolderStructure structure)
    {
        _story = story;
        _chapterContents = chapterContents;
        _imagePath = imagePath;
        _structure = structure;
    }

    private void WriteToFile(XHTMLDocument document, string path)
    {
        File.WriteAllText(path, document.ToString());
    }

    private string CreateChapterDir(int index)
    {
        return Path.Combine(_structure.ABS_OEBPS_CHAPTERS, $"chap{index}.xhtml");
    }

    public void CreareFile()
    {
        var xHTMLContent = new XHTMLContentGenerator(_story.Name, _story.Author.Name, _story.Description, string.Join(", ", _story.Categories.Select(x => x.Name)), _story.Status, _chapterContents, _imagePath);
        var contentStructure = xHTMLContent.CreateDocumentStructure();
        WriteToFile(contentStructure.Cover, _structure.ABS_F_COVER);
        WriteToFile(contentStructure.Origin, _structure.ABS_F_ORIGIN);
        WriteToFile(contentStructure.Intro, _structure.ABS_F_INTRO);
        var chapterLocal = new List<ChapterLocal>();
        var index = 0;
        foreach (var chapter in contentStructure.Chapters)
        {
            index += 1;
            var chapterDir = CreateChapterDir(index);
            WriteToFile(chapter.Document, chapterDir);
            chapterLocal.Add(new ChapterLocal(chapter.Title, chapterDir));
        }

        var opf = new OPFGenerator(_story.Name, _story.Description, _story.Author.Name, _imagePath, chapterLocal.Select(x => x.Path).ToList());
        var toc = new TOCNCXGenerator(_story.Name, opf.BookIdentifier, chapterLocal);
        var opfDoc = opf.CreateDocument();
        var tocDoc = toc.CreateDocument();
        WriteToFile(opfDoc, _structure.ABS_F_CONTENT_OPF);
        WriteToFile(tocDoc, _structure.ABS_F_TOC_NCX);
    }
}
