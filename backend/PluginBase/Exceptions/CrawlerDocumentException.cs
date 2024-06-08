using System.Runtime.Serialization;

namespace PluginBase.Exceptions
{
    public class CrawlerDocumentException : Exception
    {
        public CrawlerDocumentException()
        {
        }

        public CrawlerDocumentException(string? message) : base(message)
        {
        }

        public CrawlerDocumentException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
