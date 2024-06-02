using PluginBase.Contract;
using PluginBase.Models;
using iText.IO.Image;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.IO.Font;

namespace PDFExporter
{
    public class PDFExport : IExporter
    {
        private const string destDir = "../../../tmp/pdf/";

        public string Ext { get; } = "pdf";


        public byte[] ExportStory(StoryDetail storyDetail, List<ChapterContent> chapterContents)
        {
            var dest = $"{destDir}story.pdf";
            CreatePDFDocumentOfStory(dest, storyDetail, chapterContents);
            byte[] bytes = System.IO.File.ReadAllBytes(dest);
            File.Delete(dest);
            return bytes;
        }

        public byte[] ExportChapter(StoryDetail storyDetail, ChapterContent chapterContent)
        {
            var dest = $"{destDir}chapter.pdf";
            CreatePDFDocumentOfChapter(dest, storyDetail, chapterContent);
            byte[] bytes = System.IO.File.ReadAllBytes(dest);
            File.Delete(dest);
            return bytes;
        }
        static public void CreatePDFDocumentOfChapter(string path, StoryDetail storyDetail, ChapterContent chapterContent)
        {
            var document = CreateDocument(path);
            SetDefaultFont(ref document);
            ImageData imageData = ImageDataFactory.Create(storyDetail.ImageUrl);
            Image cover = new Image(imageData).SetAutoScale(true).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            document.Add(cover);
            SetStoryName(storyDetail.Name, ref document);
            SetAuthor(storyDetail.Author.Name, ref document);
            SetDescription(storyDetail.Description, ref document);
            SetStatus(storyDetail.Status, ref document);
            SetCategories(storyDetail.Categories, ref document);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            AddChapterContent(chapterContent, ref document);

            document.Close();
        }

        private static void AddChapterContent(ChapterContent chapterContent, ref Document document)
        {
            Paragraph chapterName = new Paragraph(chapterContent.ChapterName).SetBold().SetFontSize(18).SetDestination($"chuong{chapterContent.ChapterIndex}");
            document.Add(chapterName);

            var content = chapterContent.Content.Replace("\n\n\n", "\n\n");
            Paragraph mainContent = new Paragraph(content == null ? "" : content);
            document.Add(mainContent);
        }

        static public void CreatePDFDocumentOfStory(string path, StoryDetail storyDetail, List<ChapterContent> chapterContents)
        {
            var document = CreateDocument(path);
            SetDefaultFont(ref document);
            ImageData imageData = ImageDataFactory.Create(storyDetail.ImageUrl);
            Image cover = new Image(imageData).SetAutoScale(true).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            document.Add(cover);
            SetStoryName(storyDetail.Name, ref document);
            SetAuthor(storyDetail.Author.Name, ref document);
            SetDescription(storyDetail.Description, ref document);
            SetStatus(storyDetail.Status, ref document);
            SetCategories(storyDetail.Categories, ref document);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            SetChapterTable(ref document);
            AddChaptersTable(chapterContents, ref document);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            for (int i = 0; i < chapterContents.Count; i++)
            {
                AddChapterContent(chapterContents[i], ref document);
                if (i != chapterContents.Count - 1)
                {
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }
            }
            document.Close();
        }

        private static void AddChaptersTable(List<ChapterContent> chapterContents, ref Document document)
        {
            for (int i = 0; i < chapterContents.Count; i++)
            {
                Paragraph chapterName = new Paragraph($"{chapterContents[i].ChapterName}").SetMarginLeft(50).SetAction(PdfAction.CreateGoTo($"chuong{chapterContents[i].ChapterIndex}")).SetUnderline();
                document.Add(chapterName);
            }
        }

        private static void SetChapterTable(ref Document document)
        {
            Paragraph chapterTable = new Paragraph("Danh sách các chương").SetBold().SetFontSize(20).SetMaxWidth(250).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            document.Add(chapterTable);
        }

        private static void SetCategories(Category[] categories, ref Document document)
        {
            Paragraph category = new Paragraph("Thể loại: " + string.Join(", ", categories.Select(x => x.Name).ToList()));
            document.Add(category);
        }

        private static void SetStatus(string statusStr, ref Document document)
        {
            Paragraph status = new Paragraph("Trạng thái: " + statusStr);
            document.Add(status);
        }

        private static void SetDescription(string descriptionStr, ref Document document)
        {
            Paragraph description = new Paragraph("Mô tả: " + descriptionStr);
            document.Add(description);
        }

        private static void SetAuthor(string authorName, ref Document document)
        {
            Paragraph author = new Paragraph(authorName).SetFontColor(new DeviceRgb(50, 50, 50)).SetFontSize(20).SetHorizontalAlignment(HorizontalAlignment.RIGHT).SetHorizontalAlignment(HorizontalAlignment.RIGHT);
            document.Add(author);
        }

        private static void SetStoryName(string storyNameStr, ref Document document)
        {
            Paragraph storyName = new Paragraph(storyNameStr).SetFontSize(40).SetFontColor(new DeviceRgb(255, 100, 20)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            document.Add(storyName);
        }

        private static void SetDefaultFont(ref Document document)
        {
            var fontPath = "./fonts/vuArial.ttf";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            fontPath = Path.Combine(baseDir, fontPath);
            PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
            document.SetFont(font);
        }

        private static Document CreateDocument(string path)
        {
            FileInfo file = new FileInfo(path);
            if (!file.Directory.Exists) file.Directory.Create();

            PdfWriter writer = new PdfWriter(path);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            return document;
        }
    }
}