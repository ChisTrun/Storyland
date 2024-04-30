using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase.Models
{
    public class Categories
    {
        public string Name { get; set; }
        public string  Url { get; set; }

        public Categories(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }
    }
}
