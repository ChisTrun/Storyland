﻿using ExporterEPUB.Helpers;
using ExporterEPUB.XHTMLBuilder;

namespace ExporterEPUB.FilePrepare;

public class OPFGenerator
{
    private const string IDENTIFIER = "OPF_ID";
    private const string RIGHTS = "Copyright @ Group15 2024";
    private const string LANGUAGE = "vi";
    private const string TOC_ID = "ncx";

    private readonly string _cSS_PATH = Path.Combine("Styles", "style.css").InverseSlash();
    private readonly string _tOC_PATH = "toc.ncx";
    private readonly string _lOGO_PNG_PATH = Path.Combine("Resources", "logo.png").InverseSlash();

    public string BookIdentifier { get; }

    private readonly string _storyName;
    private readonly string _storyDescription;
    private readonly string _authorName;
    private readonly string _imagePath;
    private readonly string _coverDir;
    private readonly string _originDir;
    private readonly string _introDir;
    private readonly List<string> _chapterDirs;

    public OPFGenerator(string storyName, string storyDescription, string authorName, string imagePath, List<string> chapterDirs)
    {
        _storyName = storyName;
        _storyDescription = storyDescription;
        _authorName = authorName;
        _imagePath = Path.Combine("Images", Path.GetFileName(imagePath)).InverseSlash();
        _coverDir = Path.Combine("Chapters", Path.GetFileName(FolderStructure.COVER)).InverseSlash();
        _originDir = Path.Combine("Chapters", Path.GetFileName(FolderStructure.ORIGIN)).InverseSlash();
        _introDir = Path.Combine("Chapters", Path.GetFileName(FolderStructure.INTRO)).InverseSlash();
        _chapterDirs = chapterDirs.Select(GetChapterRelPath).ToList();
        BookIdentifier = Guid.NewGuid().ToString();
    }

    private string GetChapterRelPath(string path)
    {
        return Path.Combine("Chapters", Path.GetFileName(path)).InverseSlash();
    }

    public XHTMLDocument CreateDocument()
    {
        var xml = new XHTMLPlain("""
            <?xml version='1.0' encoding='utf-8'?>
            """);
        var package = new XHTMLElement("package", new()
        {
            {"xmlns","http://www.idpf.org/2007/opf" },
            {"unique-identifier", IDENTIFIER},
            {"version", "2.0" }
        });
        var metadata = new XHTMLElement("metadata", new()
        {
            {"xmlns:dc", "http://purl.org/dc/elements/1.1/"},
            { "xmlns:opf", "http://www.idpf.org/2007/opf"}
        });
        {
            var dc_identifier = new XHTMLElement("dc:identifier", new()
            {
                {"id", IDENTIFIER},
            }, null, BookIdentifier);
            var dc_title = new XHTMLElement("dc:title", null, null, _storyName);
            var dc_rights = new XHTMLElement("dc:rights", null, null, RIGHTS);
            var dc_creator = new XHTMLElement("dc:creator", new()
            {
                {"opf:file-as", _authorName },
                {"opf:role", "aut" }
            }, null, _authorName);
            var dc_description = new XHTMLElement("dc:description", null, null, _storyDescription);
            var dc_date = new XHTMLElement("dc:date", new()
            {
                {"opf:event", "publication" }
            }, null, DateTime.Today.ToString("yyyy-MM-dd"));
            var dc_language = new XHTMLElement("dc:language", null, null, LANGUAGE);
            var metaCover = new XHTMLElementInline("meta", new()
            {
                {"name", "cover" },
                {"content", _imagePath}
            });
            var metaSigil = new XHTMLElementInline("meta", new()
            {
                {"name", "Sigil version" },
                {"content",  "0.5.0"}
            });
            metadata.AddChild(dc_identifier);
            metadata.AddChild(dc_title);
            metadata.AddChild(dc_rights);
            metadata.AddChild(dc_creator);
            metadata.AddChild(dc_description);
            metadata.AddChild(dc_date);
            metadata.AddChild(dc_language);
            metadata.AddChild(metaCover);
            metadata.AddChild(metaSigil);
        }
        var manifest = new XHTMLElement("manifest", null, new()
        {
            // predefined files
            ItemGen(_tOC_PATH, TOC_ID, "application/x-dtbncx+xml"),
            ItemGen(_cSS_PATH, "css", "text/css"),
            ItemGen(_lOGO_PNG_PATH, "logo.png", "image/png"),
            ItemGen(_imagePath, "cover", "image/jpeg"),
            ItemGen(_coverDir, Path.GetFileName(_coverDir), "application/xhtml+xml"),
            ItemGen(_originDir, Path.GetFileName(_originDir), "application/xhtml+xml"),
            ItemGen(_introDir, Path.GetFileName(_introDir), "application/xhtml+xml")
        });
        foreach (var chapterDir in _chapterDirs)
        {
            manifest.AddChild(ItemGen(chapterDir, Path.GetFileName(chapterDir), "application/xhtml+xml"));
        }
        var spine = new XHTMLElement("spine", new()
        {
            {"toc", TOC_ID }
        }, new()
        {
            new XHTMLElementInline("itemref", new() {{ "idref", Path.GetFileName(_coverDir) } }),
            new XHTMLElementInline("itemref", new() {{ "idref", Path.GetFileName(_originDir) } }),
            new XHTMLElementInline("itemref", new() {{ "idref", Path.GetFileName(_introDir) } }),
        });
        foreach (var chapterDir in _chapterDirs)
        {
            spine.AddChild(new XHTMLElementInline("itemref", new() {
                { "idref", Path.GetFileName(chapterDir) }
            }));
        }
        var guide = new XHTMLElement("guide", null, new()
        {
            new XHTMLElementInline("reference", new()
            {
                {"href", _coverDir},
                {"title", "Cover" },
                {"type", "cover" }
            })
        });
        package.AddChild(metadata);
        package.AddChild(manifest);
        package.AddChild(spine);
        package.AddChild(guide);

        var doc = new XHTMLDocument(new()
        {
            xml, package
        });
        return doc;
    }

    private static XHTMLElementInline ItemGen(string href, string id, string media_type)
    {
        return new XHTMLElementInline("item", new()
            {
                {"href", href},
                {"id", id},
                {"media-type", media_type}
            });
    }
}
