using backend.Domain.Entities;
using ExporterEPUB.FilePrepare;
using ExporterEPUB.Helpers;

namespace ExporterEPUB
{
    public class ExporterEPUBServe : IDisposable
    {
        private readonly FolderStructure _structure;
        private readonly StoryDetail _storyDetail;
        private readonly List<ChapterContent> _chapterContents;

        public ExporterEPUBServe(StoryDetail storyDetail, List<ChapterContent> chapterContents)
        {
            _storyDetail = storyDetail;
            _chapterContents = chapterContents;
            var gui = $"Epub-{Guid.NewGuid()}-{DateTime.Now:dd-MM-yyyy}";
            var dir = Directory.CreateDirectory(gui);
            _structure = new FolderStructure(dir.FullName, FolderStructure.EPUBRESOURCE);
        }

        public byte[] ExportEpub()
        {
            var imgPath = FileGenerator.SaveImage(_storyDetail.ImageURL, _structure);
            var allGen = new AllGenerator(_storyDetail, _chapterContents, imgPath, _structure);
            allGen.CreareFile();
            var zipFileBytes = FileZipper.Zip(_structure.BaseAbsoluteDir);
            return zipFileBytes;
        }

        public void Dispose()
        {
            Directory.Delete(_structure.BaseAbsoluteDir, true);
        }
    }
}
