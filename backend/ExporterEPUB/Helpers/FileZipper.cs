using ICSharpCode.SharpZipLib.Zip;

namespace ExporterEPUB.Helpers
{
    public class FileZipper
    {
        public static byte[] Zip(string absPath)
        {
            FastZip z = new()
            {
                CreateEmptyDirectories = true
            };
            var s = new MemoryStream();
            z.CreateZip(s, absPath, true, "", "");
            return s.ToArray();
        }
    }
}
