using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase.Models
{
    public class Story
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string? ImageUrl {  get; set; }

        public Story(string name, string url, string? img = null)
        {
            this.Name = name;
            this.Url = url;
            this.ImageUrl = img;
        }
    }
}
