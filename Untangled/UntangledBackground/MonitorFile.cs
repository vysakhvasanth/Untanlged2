using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UntangledBackground
{
    public class MonitorFile 
    {
        private FileSystemWatcher fileWatcher;
        private string filename;
        public MonitorFile(string filename)
        {
            this.filename = filename;
            fileWatcher = new FileSystemWatcher(filename);
        }

        public void StartFileMonitor()
        {
            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = Path.GetDirectoryName(filename);
            fileWatcher.Filter = Path.GetFileName(filename);
            fileWatcher.NotifyFilter = NotifyFilters.LastAccess |
                         NotifyFilters.LastWrite |
                         NotifyFilters.FileName |
                         NotifyFilters.DirectoryName;

            //events
            

            fileWatcher.EnableRaisingEvents = true;
        }
    }
}
