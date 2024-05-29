namespace ExporterEPUB.Model
{
    public class ChapterLocal
    {
        public ChapterLocal(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; }
        public string Path { get; }
    }
}
