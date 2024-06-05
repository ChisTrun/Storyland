using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase.Utils
{
    public delegate void VoidDelegate();

    public class TryCatch
    {
        public static void Try(VoidDelegate v)
        {
            try
            {
                v();
            }
            catch { }
        }
    }
}
