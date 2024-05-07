using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase.Models
{
    public class Chapter
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public Chapter(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
