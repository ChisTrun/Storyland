namespace ExporterEPUB.XHTMLBuilder.Content;

public class ChapterXHTML : IContentXHTML
{
    private readonly string _title;
    private readonly string _content;

    public ChapterXHTML(string title, string content)
    {
        _title = title;
        _content = content;
    }

    public LayoutXHTML SetContent(LayoutXHTML layout)
    {
        var chapterTitleEl = new XHTMLElement("h1", null, null, _title);
        var chapterContentEl = new XHTMLElement("p", null, null, _content);
        layout.Body.AddChild(chapterTitleEl);
        layout.Body.AddChild(chapterContentEl);
        return layout;
    }
}
