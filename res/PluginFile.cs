using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayModsManager.res
{
    /* файл который относится к плагину */
    class PluginFile
    {
        public string path;
        public string plugin;

        public PluginFile(string _plugin, string _path)
        {
            path = _path;
            plugin = _plugin;
        }
    }
}
