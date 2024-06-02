using PluginBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase.Contract
{
    public interface IExporter
    {
        public string Ext { get; }
        byte[] ExportStory(StoryDetail storyDetail, List<ChapterContent> chapterContents);
        byte[] ExportChapter(StoryDetail storyDetail, ChapterContent chapterContent);
    }
}
