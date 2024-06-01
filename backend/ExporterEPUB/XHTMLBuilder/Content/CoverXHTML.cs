namespace ExporterEPUB.XHTMLBuilder.Content;

public class CoverXHTML : IContentXHTML
{
    private readonly string _imagePath;

    public CoverXHTML(string imagePath)
    {
        _imagePath = imagePath;
    }

    public LayoutXHTML SetContent(LayoutXHTML layout)
    {
        var css = new XHTMLElement("style");
        css.AddText("""
            .cover-container {
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100%;
                /* Adjust this if needed */
                overflow: hidden;
            }

            .cover-container img {
                height: 90vh;
                max-width: 60vw;
            }
            """);
        layout.Head.AddChild(css);


        var cover = new XHTMLElementInline("img", new Dictionary<string, string>()
        {
            {"alt", "Cover" },
            {"src", _imagePath }
        });
        var container = new XHTMLElement("div");
        container.AddAttribute("class", "cover-container");
        container.AddChild(cover);
        layout.Body.AddChild(container);
        return layout;
    }
}
