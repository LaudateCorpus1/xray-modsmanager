using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayModsManager.res
{
    /* класс для контроля конфликтов */
    class FilesController
    {
        private List<PluginFile> lst = new List<PluginFile>();

        // добавить файл в список
        public void addFile(string plugin, string file)
        {
            lst.Add(new PluginFile(plugin, file));
        }

        // найти файл в списке
        public PluginFile findFile(string file)
        {
            foreach(var item in lst)
            {
                if (item.path == file)
                    return item;
            }

            return null;
        }
    }
}
