using ExporterEPUB.Helpers;
using ExporterEPUB.Model;
using ExporterEPUB.XHTMLBuilder;

namespace ExporterEPUB.FilePrepare
{
    public class TOCNCXGenerator
    {
        private readonly string _storyName;
        private readonly string _bookIdentifier;
        private readonly List<ChapterLocal> _chapters;

        public TOCNCXGenerator(string storyName, string bookIdentifier, List<ChapterLocal> chapters)
        {
            _storyName = storyName;
            _bookIdentifier = bookIdentifier;
            _chapters = chapters;
        }

        private string GetChapterRelPath(string path)
        {
            return Path.Combine("Chapters", Path.GetFileName(path)).InverseSlash();
        }

        public XHTMLDocument CreateDocument()
        {
            var xml = new XHTMLPlain("""
                <?xml version="1.0" encoding="utf-8"?>
                """);
            XHTMLElementInline MetaGen(string name, string content) => new XHTMLElementInline("meta", new()
            {
                {"name", name },
                {"content", content}
            });
            var ncx = new XHTMLElement("ncx", new()
            {
                {"xmlns", "http://www.daisy.org/z3986/2005/ncx/" },
                {"version", "2005-1" }
            }, new()
            {
                new XHTMLElement("head", null, new()
                {
                    MetaGen("dtb:uid", _bookIdentifier),
                    MetaGen("dtb:depth", "1"),
                    MetaGen("dtb:totalPageCount", "0"),
                    MetaGen("dtb:maxPageNumber", "0"),
                }),
                new XHTMLElement("docTitle", null, new()
                {
                    new XHTMLElement("text", null, null, _storyName)
                }),
            });
            var navMap = new XHTMLElement("navMap");
            {
                XHTMLElement NavPointGen(string id, int playOrder, ChapterLocal chapter) => new("navPoint", new()
                {
                    {"id", id },
                    {"playOrder", playOrder.ToString() }
                }, new()
                {
                    new XHTMLElement("navLabel", null, new()
                    {
                        new XHTMLElement("text", null, null, chapter.Name)
                    }),
                    new XHTMLElementInline("content", new(){{"src", GetChapterRelPath(chapter.Path)}})
                });
                int id = 1;
                foreach (var chapter in _chapters)
                {
                    navMap.AddChild(NavPointGen($"nav{id}", id, chapter));
                    id += 1;
                }
            }
            ncx.AddChild(navMap);
            var doc = new XHTMLDocument(new()
            {
                xml, ncx
            });
            return doc;
            //File.WriteAllText(EPUBStructure.ToAbsolute( EPUBStructure.f_TOC_NCX), doc.ToString());
        }
    }
}
