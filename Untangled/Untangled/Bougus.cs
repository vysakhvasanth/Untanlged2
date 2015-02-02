using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardControlLibrary;

namespace Untangled
{
    public class Bougus
    {
        private FileSystemWatcher _fMonitor;
        private MainWindow mnWindow;
        public Bougus(MainWindow mnWindow, string _filePath)
        {
            this.mnWindow = mnWindow;
            _fMonitor = new FileSystemWatcher
            {
                Path = _filePath,
                Filter = "*.*",
                NotifyFilter = NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.FileName |
                               NotifyFilters.DirectoryName,
                //IncludeSubdirectories = true
            };

            _fMonitor.Created += _fMonitor_Created;
            _fMonitor.EnableRaisingEvents = true;

            
        }

        private void _fMonitor_Created(object sender, FileSystemEventArgs e)
        {
            
            //BackgroundThread for adding the cards
            var bg = new BackgroundWorker();
            bg.DoWork += (i, j) => mnWindow.StackMain.Dispatcher.Invoke((() => AddCard(mnWindow, e.Name, e.FullPath,e.ChangeType.ToString())));
            bg.RunWorkerAsync();
        }

        private void AddCard(MainWindow mnWindow, string name, string fullpath, string activity)
        {

            var info = new CardProperties
            {
                Name = name,
                Path = fullpath,
                Activity = activity,
                FileType = FileType.File,
                Progress = 100
            };


            var card = new AwesomeCard()
            {
                DataContext = info,
                CardProperties = info,
                Width = 490,
                Height = 150
            };

            mnWindow.StackMain.Children.Add(card);
            mnWindow.StackMain.Children.Add(card);
            mnWindow.StackMain.Children.Add(card);
        }
    }
}
