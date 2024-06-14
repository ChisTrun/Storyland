using System.Text;

namespace ExporterEPUB.XHTMLBuilder;

public abstract class XHTMLNode
{
    public override abstract string ToString();
}

public class XHTMLElement : XHTMLNode
{
    public string Name { get; }
    public Dictionary<string, string> Attributes { get; }
    public List<XHTMLNode> Children { get; }

    public XHTMLElement(string element, Dictionary<string, string>? attributes = null, List<XHTMLNode>? children = null, string? text = null)
    {
        Name = element;
        Attributes = attributes ?? new();
        Children = children ?? new();
        if (text != null)
        {
            Children.Add(new XHTMLText(text));
        }
    }

    public void AddText(string text) => Children.Add(new XHTMLText(text));
    public void AddChild(XHTMLNode child) => Children.Add(child);
    public void AddAttribute(string key, string value) => Attributes.Add(key, value);

    public void InsertText(string text, int index) => Children.Insert(index, new XHTMLText(text));
    public void InsertChild(XHTMLNode child, int index) => Children.Insert(index, child);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"<{Name}");
        if (Attributes.Count > 0)
        {
            sb.Append(' ').AppendJoin(' ', Attributes.Select(x => $"{x.Key}=\"{x.Value}\""));
        }
        sb.AppendLine(">");
        foreach (var child in Children)
        {
            sb.AppendLine(child.ToString());
        }
        sb.AppendLine($"</{Name}>");
        return sb.ToString();
    }
}

/// <summary>
/// Short tag: <image ... />
/// </summary>
public class XHTMLElementInline : XHTMLNode
{
    public string Name { get; }
    public Dictionary<string, string> Attributes { get; }
    public XHTMLElementInline(string name, Dictionary<string, string>? attributes = null)
    {
        Name = name;
        Attributes = attributes ?? new();
    }
    public void AddAttribute(string key, string value) => Attributes.Add(key, value);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"<{Name}");
        if (Attributes.Count > 0)
        {
            sb.Append(' ').AppendJoin(' ', Attributes.Select(x => $"{x.Key}=\"{x.Value}\""));
        }
        sb.AppendLine(" />");
        return sb.ToString();
    }
}

/// <summary>
/// Used for special tags: <!DOCTYPE ...>, <?xml ...> or long templates
/// </summary>
public class XHTMLPlain : XHTMLNode
{
    public string Text { get; }

    public XHTMLPlain(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return Text;
    }
}

public class XHTMLText : XHTMLNode
{
    public string Text { get; }

    public XHTMLText(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return Text;
    }
}

/// <summary>
/// Container
/// </summary>
public class XHTMLDocument : XHTMLNode
{
    public List<XHTMLNode> Children { get; }

    public XHTMLDocument(List<XHTMLNode>? children = null)
    {
        Children = children ?? new();
    }

    public void AddChild(XHTMLNode child) => Children.Add(child);

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var child in Children)
        {
            sb.AppendLine(child.ToString());
        }
        return sb.ToString();
    }
}
