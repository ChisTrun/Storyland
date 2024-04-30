using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase.Models
{
    public  class ChapterContent
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string? NextChapUrl { get; set; }
        public string? PreChapUrl { get; set; }

        public ChapterContent(string name, string content, string? next = null, string? pre = null)
        {
            this.Name = name;
            this.Content = content;
            this.NextChapUrl = next;
            this.PreChapUrl = pre;
        }
    }
}
