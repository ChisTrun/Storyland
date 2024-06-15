using Backend.Domain.Contract;
using Backend.Domain.Entities;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PDFExporter
{
    public class PDFExport : IExporter
    {
        private const string DESTDIR = "../../../tmp/pdf/";

        public string Name => "PDF";

        public string Extension => "pdf";

        public byte[] ExportStory(StoryDetail storyDetail, IEnumerable<ChapterContent> chapters)
        {
            var chapterContents = chapters.ToList();
            var dest = $"{DESTDIR}story.pdf";
            CreatePDFDocumentOfStory(dest, storyDetail, chapterContents);
            var bytes = File.ReadAllBytes(dest);
            File.Delete(dest);
            return bytes;
        }

        public byte[] ExportChapter(StoryDetail storyDetail, ChapterContent chapterContent)
        {
            var dest = $"{DESTDIR}chapter.pdf";
            CreatePDFDocumentOfChapter(dest, storyDetail, chapterContent);
            var bytes = System.IO.File.ReadAllBytes(dest);
            File.Delete(dest);
            return bytes;
        }
        static public void CreatePDFDocumentOfChapter(string path, StoryDetail storyDetail, ChapterContent chapterContent)
        {
            var document = CreateDocument(path);
            SetDefaultFont(ref document);
            var imageData = ImageDataFactory.Create(storyDetail.ImageURL);
            var cover = new Image(imageData).SetAutoScale(true).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            document.Add(cover);
            SetStoryName(storyDetail.Name, ref document);
            SetAuthor(storyDetail.Author.Name, ref document);
            SetDescription(storyDetail.Description, ref document);
            SetStatus(storyDetail.Status, ref document);
            SetCategories(storyDetail.Categories.ToArray(), ref document);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            AddChapterContent(chapterContent, ref document);

            document.Close();
        }

        private static void AddChapterContent(ChapterContent chapterContent, ref Document document)
        {
            var chapterName = new Paragraph(chapterContent.Name).SetBold().SetFontSize(18).SetDestination($"chuong{chapterContent.Index}");
            document.Add(chapterName);

            var content = chapterContent.Content.Replace("\n\n\n", "\n\n");
            var mainContent = new Paragraph(content ?? "");
            document.Add(mainContent);
        }

        static public void CreatePDFDocumentOfStory(string path, StoryDetail storyDetail, List<ChapterContent> chapterContents)
        {
            var document = CreateDocument(path);
            SetDefaultFont(ref document);
            var imageData = ImageDataFactory.Create(storyDetail.ImageURL);
            var cover = new Image(imageData).SetAutoScale(true).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            document.Add(cover);
            SetStoryName(storyDetail.Name, ref document);
            SetAuthor(storyDetail.Author.Name, ref document);
            SetDescription(storyDetail.Description, ref document);
            SetStatus(storyDetail.Status, ref document);
            SetCategories(storyDetail.Categories.ToArray(), ref document);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            SetChapterTable(ref document);
            AddChaptersTable(chapterContents, ref document);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            for (var i = 0; i < chapterContents.Count; i++)
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
            for (var i = 0; i < chapterContents.Count; i++)
            {
                var chapterName = new Paragraph($"{chapterContents[i].Name}").SetMarginLeft(50).SetAction(PdfAction.CreateGoTo($"chuong{chapterContents[i].Index}")).SetUnderline();
                document.Add(chapterName);
            }
        }

        private static void SetChapterTable(ref Document document)
        {
            var chapterTable = new Paragraph("Danh sách các chương").SetBold().SetFontSize(20).SetMaxWidth(250).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            document.Add(chapterTable);
        }

        private static void SetCategories(Category[] categories, ref Document document)
        {
            var category = new Paragraph("Thể loại: " + string.Join(", ", categories.Select(x => x.Name).ToList()));
            document.Add(category);
        }

        private static void SetStatus(string statusStr, ref Document document)
        {
            var status = new Paragraph("Trạng thái: " + statusStr);
            document.Add(status);
        }

        private static void SetDescription(string descriptionStr, ref Document document)
        {
            var description = new Paragraph("Mô tả: " + descriptionStr);
            document.Add(description);
        }

        private static void SetAuthor(string authorName, ref Document document)
        {
            var author = new Paragraph(authorName).SetFontColor(new DeviceRgb(50, 50, 50)).SetFontSize(20).SetHorizontalAlignment(HorizontalAlignment.RIGHT).SetHorizontalAlignment(HorizontalAlignment.RIGHT);
            document.Add(author);
        }

        private static void SetStoryName(string storyNameStr, ref Document document)
        {
            var storyName = new Paragraph(storyNameStr).SetFontSize(40).SetFontColor(new DeviceRgb(255, 100, 20)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            document.Add(storyName);
        }

        private static void SetDefaultFont(ref Document document)
        {
            var fontPath = "./fonts/vuArial.ttf";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            fontPath = Path.Combine(baseDir, fontPath);
            var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
            document.SetFont(font);
        }

        private static Document CreateDocument(string path)
        {
            var file = new FileInfo(path);
            if (!file.Directory!.Exists)
            {
                file.Directory.Create();
            }

            var writer = new PdfWriter(path);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            return document;
        }
    }
}