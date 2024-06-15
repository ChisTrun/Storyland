using Backend.Domain.Contract;
using Backend.Domain.Entities;
using ExporterEPUB;
using Newtonsoft.Json;
using System.Diagnostics;

namespace PRCExporter
{
    public class PRCExport : IExporter
    {
        private readonly string _tempDir = "Resources/Calibre/TempFile";
        private readonly string _fileName = "story";

        public string Name => "PRC";

        public string Extension => "prc";

        public byte[] ExportStory(StoryDetail story, IEnumerable<ChapterContent> chapters)
        {
            byte[] byteStream;
            using (var serve = new ExporterEPUBServe(story, chapters))
            {
                byteStream = serve.ExportEpub();
            }

            if (!Directory.Exists(_tempDir))
            {
                Directory.CreateDirectory(_tempDir);
            }

            // Create temporary files
            string tempEpubPath = Path.Combine(_tempDir, _fileName + ".epub");
            File.WriteAllBytes(tempEpubPath, byteStream);
            string outputPath = Path.Combine(_tempDir, _fileName + ".mobi");

            // Convert epub to mobi using Calibre
            string calibrePath = GetCalibrePath();
            ConvertToMobi(calibrePath, tempEpubPath, outputPath);

            // Change Extension to .prc
            string tempPrcPath = Path.ChangeExtension(outputPath, ".prc");
            File.Move(outputPath, tempPrcPath);

            // Read converted prc file as byte stream
            byte[] outputByteStream = File.ReadAllBytes(tempPrcPath);

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

            if (!Directory.Exists(_tempDir))
            {
                Directory.CreateDirectory(_tempDir);
            }

            // Create temporary files
            string tempEpubPath = Path.Combine(_tempDir, _fileName + ".epub");
            File.WriteAllBytes(tempEpubPath, byteStream);
            string outputPath = Path.Combine(_tempDir, _fileName + ".mobi");

            // Convert epub to mobi using Calibre
            string calibrePath = GetCalibrePath();
            ConvertToMobi(calibrePath, tempEpubPath, outputPath);

            // Change Extension to .prc
            string tempPrcPath = Path.ChangeExtension(outputPath, ".prc");
            File.Move(outputPath, tempPrcPath);

            // Read converted prc file as byte stream
            byte[] outputByteStream = File.ReadAllBytes(tempPrcPath);

            CleanTempFile(tempEpubPath, tempPrcPath);

            return outputByteStream;
        }

        public static string GetCalibrePath()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Calibre/config.json");
            if (!File.Exists(configFilePath))
            {
                throw new Exception("Configuration file not found!");
            }

            string configJson = File.ReadAllText(configFilePath);
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
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Error during conversion: {error}");
            }
        }
    }
}
