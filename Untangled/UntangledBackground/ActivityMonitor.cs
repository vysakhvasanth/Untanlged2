using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;

namespace UntangledBackground
{
    public class ActivityMonitor
    {
        private FileSystemWatcher _fMonitor;
        private string _filePath;
        private MainWindow mnWindow;
        public ObservableCollection<FileDetails> FileList { get; private set; }
        private static object _syncLock = new object(); //lock object for synchronization

        public delegate void UpdateDataGridDelegate();
        private UpdateDataGridDelegate _updateDelegate;
       
        public ActivityMonitor(MainWindow mnWindow, string filePath)
        {
            this.mnWindow = mnWindow;
            this._filePath = filePath;
            _updateDelegate = GridUpdate;
            FileList = new ObservableCollection<FileDetails>();
            BindingOperations.EnableCollectionSynchronization(FileList,_syncLock);
        }

        public void StartMonitor()
        {
            _fMonitor = new FileSystemWatcher();
            _fMonitor.Path = _filePath;
            _fMonitor.Filter = "*.*";
            _fMonitor.NotifyFilter = NotifyFilters.LastAccess |
                         NotifyFilters.LastWrite |
                         NotifyFilters.FileName |
                         NotifyFilters.DirectoryName;

            _fMonitor.IncludeSubdirectories = true;
            _fMonitor.Changed += _fMonitor_Changed;
            _fMonitor.Deleted += _fMonitor_Deleted;
            _fMonitor.Created += _fMonitor_Created;
            _fMonitor.Renamed += _fMonitor_Renamed;

            _fMonitor.EnableRaisingEvents = true;

        }

        void _fMonitor_Renamed(object sender, RenamedEventArgs e)
        {

        }

        void _fMonitor_Created(object sender, FileSystemEventArgs e)
        {
            FileList.Add(new FileDetails()
            {
                name = e.Name + " (Created)",
                path = e.FullPath
            });

            CrossCall(mnWindow.MonitorView, _updateDelegate);
        }

        void _fMonitor_Deleted(object sender, FileSystemEventArgs e)
        {
            FileList.Add(new FileDetails()
            {
                name = e.Name + " (Deleted)",
                path = e.FullPath
            });
     
            CrossCall(mnWindow.MonitorView, _updateDelegate);
        }

        private DateTime _timestamp;
        private bool _isFirstWrite = true;
       
        //Not required as for now, focus on created and deleted events
        void _fMonitor_Changed(object sender, FileSystemEventArgs e)
        {
            //var currentTimeStamp = File.GetLastWriteTime(e.FullPath);
            //if ((currentTimeStamp.Second - _timestamp.Second) > _DELAY || _isFirstWrite)
            //{
            //    FileList.Add(new FileDetails()
            //    {
            //        name = e.Name + " (Modified)",
            //        path = e.FullPath
            //    });
            //    _isFirstWrite = false;
            //}

            //CrossCall(mnWindow.MonitorView, _updateDelegate);
            //_timestamp = File.GetLastWriteTime(e.FullPath);
        }

        public void GridUpdate()
        {
            mnWindow.MonitorView.ItemsSource = FileList;
        }

        public void CrossCall(object obj, UpdateDataGridDelegate function)
        {
            var cr = obj as Control;
            if (cr != null)
            {
                cr.Dispatcher.Invoke(() => function());
            }
        }

    }
}
