namespace ExporterEPUB.XHTMLBuilder;

/// <summary>
/// Structures how an Epub XHTML document can be
/// </summary>
public class LayoutXHTML
{
    public LayoutXHTML(string title, string cssLink)
    {
        XmlID = new XHTMLPlain("""
            <?xml version='1.0' encoding='utf-8'?>
            """);
        Doctype = new XHTMLPlain("""
            <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
            """);
        ;
        Html = new XHTMLElement("html", new Dictionary<string, string>()
        {
            {"xmlns", "http://www.w3.org/1999/xhtml" },
            {"xmlns:epub", "http://www.idpf.org/2007/ops" },
            {"xml:lang", "vn" },
            {"lang", "vn" }
        });
        Head = new XHTMLElement("head");
        Body = new XHTMLElement("body");
        Html.AddChild(Head);
        Html.AddChild(Body);

        var titleTag = new XHTMLElement("title");
        titleTag.AddText(title);
        Head.AddChild(titleTag);

        var cssLinkTag = new XHTMLElementInline("link", new Dictionary<string, string>()
        {
            {"href", cssLink},
            {"rel", "stylesheet"},
            {"type" , "text/css"},
        });
        Head.AddChild(cssLinkTag);
    }

    public XHTMLNode XmlID { get; }
    public XHTMLNode Doctype { get; }
    public XHTMLElement Html { get; }
    public XHTMLElement Head { get; }
    public XHTMLElement Body { get; }

    public XHTMLDocument GetXHTML()
    {
        return new XHTMLDocument(new List<XHTMLNode>() {
            XmlID,
            Doctype,
            Html
        });
    }
}