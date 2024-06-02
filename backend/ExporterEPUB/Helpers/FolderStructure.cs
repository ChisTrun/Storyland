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

    public string ABS_META_INF => Path.Combine(BaseAbsoluteDir, META_INF);
    public string ABS_MIMETYPE => Path.Combine(BaseAbsoluteDir, MIMETYPE);
    public string ABS_OEBPS => Path.Combine(BaseAbsoluteDir, OEBPS);

    public string ABS_OEBPS_IMAGES => Path.Combine(ABS_OEBPS, OEBPS_IMAGES);
    public string ABS_OEBPS_STYLES => Path.Combine(ABS_OEBPS, OEBPS_STYLES);
    public string ABS_OEBPS_CHAPTERS => Path.Combine(ABS_OEBPS, OEBPS_CHAPTERS);
    public string ABS_OEBPS_RESOURCES => Path.Combine(ABS_OEBPS, OEBPS_RESOURCES);
    public string ABS_F_CONTENT_OPF => Path.Combine(ABS_OEBPS, F_OEBPS_CONTENT_OPF);
    public string ABS_F_TOC_NCX => Path.Combine(ABS_OEBPS, F_OEBPS_TOC_NCX);

    public string ABS_F_COVER => Path.Combine(ABS_OEBPS_CHAPTERS, COVER);
    public string ABS_F_ORIGIN => Path.Combine(ABS_OEBPS_CHAPTERS, ORIGIN);
    public string ABS_F_INTRO => Path.Combine(ABS_OEBPS_CHAPTERS, INTRO);

    public static string BIN => AppDomain.CurrentDomain.BaseDirectory;
    public static string EPUBRESOURCE => Path.Combine(BIN, "EPUBResource");


    public static readonly string META_INF = "META-INF";
    public static readonly string MIMETYPE = "mimetype";
    public static readonly string OEBPS = "OEBPS";

    public static readonly string OEBPS_IMAGES = "Images";
    public static readonly string OEBPS_STYLES = "Styles";
    public static readonly string OEBPS_CHAPTERS = "Chapters";
    public static readonly string OEBPS_RESOURCES = "Resources";
    public static readonly string F_OEBPS_CONTENT_OPF = "content.opf";
    public static readonly string F_OEBPS_TOC_NCX = "toc.ncx";

    public static readonly string COVER = "cover.xhtml";
    public static readonly string INTRO = "intro.xhtml";
    public static readonly string ORIGIN = "origin.xhtml";
}
