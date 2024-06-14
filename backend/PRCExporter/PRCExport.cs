using backend.Domain.Contract;
using backend.Domain.Entities;
using ExporterEPUB;
using Newtonsoft.Json;
using System.Diagnostics;

namespace PRCExporter
{
    public class PRCExport : IExporter
    {
        private readonly string tempDir = "Resources/Calibre/TempFile";
        private readonly string fileName = "story";

        public string Name => "PRC";

        public string Extension => "prc";

        public byte[] ExportStory(StoryDetail story, IEnumerable<ChapterContent> chapters)
        {
            byte[] byteStream;
            using (var serve = new ExporterEPUBServe(story, chapters))
            {
                byteStream = serve.ExportEpub();
            }

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            // Create temporary files
            var tempEpubPath = Path.Combine(tempDir, fileName + ".epub");
            File.WriteAllBytes(tempEpubPath, byteStream);
            var outputPath = Path.Combine(tempDir, fileName + ".mobi");

            // Convert epub to mobi using Calibre
            var calibrePath = GetCalibrePath();
            ConvertToMobi(calibrePath, tempEpubPath, outputPath);

            // Change Extension to .prc
            var tempPrcPath = Path.ChangeExtension(outputPath, ".prc");
            File.Move(outputPath, tempPrcPath);

            // Read converted prc file as byte stream
            var outputByteStream = File.ReadAllBytes(tempPrcPath);

            CleanTempFile(tempEpubPath, tempPrcPath);

            return outputByteStream;
        }

        public byte[] ExportChapter(StoryDetail story, ChapterContent chapters)
        {
            byte[] byteStream;
            using (var serve = new ExporterEPUBServe(story, [chapters]))
            {
                byteStream = serve.ExportEpub();
            }

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            // Create temporary files
            var tempEpubPath = Path.Combine(tempDir, fileName + ".epub");
            File.WriteAllBytes(tempEpubPath, byteStream);
            var outputPath = Path.Combine(tempDir, fileName + ".mobi");

            // Convert epub to mobi using Calibre
            var calibrePath = GetCalibrePath();
            ConvertToMobi(calibrePath, tempEpubPath, outputPath);

            // Change Extension to .prc
            var tempPrcPath = Path.ChangeExtension(outputPath, ".prc");
            File.Move(outputPath, tempPrcPath);

            // Read converted prc file as byte stream
            var outputByteStream = File.ReadAllBytes(tempPrcPath);

            CleanTempFile(tempEpubPath, tempPrcPath);

            return outputByteStream;
        }

        public static string GetCalibrePath()
        {
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Calibre/config.json");
            if (!File.Exists(configFilePath))
            {
                throw new Exception("Configuration file not found!");
            }

            var configJson = File.ReadAllText(configFilePath);
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(configJson);
            return config == null ? throw new Exception("Configuration path not found!") : config["calibrePath"];
        }

        public static void CleanTempFile(string tempEpubPath, string tempPrcPath)
        {
            File.Delete(tempEpubPath);
            File.Delete(tempPrcPath);
        }

        public static void ConvertToMobi(string calibrePath, string tempEpubPath, string outputPath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = calibrePath,
                    Arguments = $"{tempEpubPath} {outputPath}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            process.StandardOutput.ReadToEnd(); // Read and ignore the standard output
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Error during conversion: {error}");
            }
        }
    }
}
