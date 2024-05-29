namespace ExporterEPUB.Helpers;


public class FolderStructure
{
    public FolderStructure(string baseAbsDir, string sourceAbsDir)
    {
        BaseAbsoluteDir = baseAbsDir;
        FileGenerator.CopyDirectory(sourceAbsDir, baseAbsDir);
        Directory.CreateDirectory(ABS_OEBPS_IMAGES);
        Directory.CreateDirectory(ABS_OEBPS_CHAPTERS);
    }

    public string BaseAbsoluteDir { get; private set; }

    public string ABS_META_INF => Path.Combine(BaseAbsoluteDir, "META-INF");
    public string ABS_MIMETYPE => Path.Combine(BaseAbsoluteDir, "mimetype");
    public string ABS_OEBPS => Path.Combine(BaseAbsoluteDir, "OEBPS");

    public string ABS_OEBPS_IMAGES => Path.Combine(ABS_OEBPS, "Images");
    public string ABS_OEBPS_STYLES => Path.Combine(ABS_OEBPS, "Styles");
    public string ABS_OEBPS_CHAPTERS => Path.Combine(ABS_OEBPS, "Chapters");
    public string ABS_OEBPS_RESOURCES => Path.Combine(ABS_OEBPS, "Resources");
    public string ABS_F_CONTENT_OPF => Path.Combine(ABS_OEBPS, "content.opf");
    public string ABS_F_TOC_NCX => Path.Combine(ABS_OEBPS, "toc.ncx");

    public string ABS_F_COVER => Path.Combine(ABS_OEBPS_CHAPTERS, "cover.xhtml");
    public string ABS_F_INTRO => Path.Combine(ABS_OEBPS_CHAPTERS, "intro.xhtml");

    public static string BIN => AppDomain.CurrentDomain.BaseDirectory;
    public static string EPUBRESOURCE => Path.Combine(BIN, "EPUBResource");

    public static readonly string META_INF = "META-INF";
    public static readonly string MIMETYPE = "mimetype";
    public static readonly string OEBPS = "OEBPS";
    public static readonly string OEBPS_IMAGES = "Images";
    public static readonly string OEBPS_STYLES = "Styles";
    public static readonly string OEBPS_CHAPTERS = "Chapters";
    public static readonly string OEBPS_RESOURCES = "Resources";

    public static readonly string F_CONTENT_OPF = "content.opf";
    public static readonly string F_TOC_NCX = "toc.ncx";

    public static readonly string COVER = "cover.xhtml";
    public static readonly string INTRO = "intro.xhtml";
}
