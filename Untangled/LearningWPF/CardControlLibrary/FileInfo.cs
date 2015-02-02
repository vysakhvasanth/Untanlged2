using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardControlLibrary
{
    public enum FileType
    {
        File,
        Directory,
        Unknown
    }


    public class FileInfo : INotifyPropertyChanged
    {
        private string name;
        private string path;
        private string activity;
        private FileType filetype;
        private int progress;
        private string progressindicator;
        private string timeunit = "M";

        public string Name
        {
            get { return name; }
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Path
        {
            get { return path; }
            set
            {
                if (value == path) return;
                path = value;
                OnPropertyChanged("Path");
            }
        }
        public string Activity
        {
            get { return activity; }
            set
            {
                if (value == activity) return;
                activity = value;
                OnPropertyChanged("Activity");
            }
        }
        public FileType FileType
        {
            get { return filetype; }
            set
            {
                if (value == filetype) return;
                filetype = value;
                OnPropertyChanged("FileType");
            }
        }

        public int Progress
        {
            get { return progress; }
            set
            {
                if (value == progress) return;
                if (value < 0) return;
                ProgressIndicator = value.ToString();
                progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public string ProgressIndicator
        {
            get { return progressindicator + string.Format(" {0}",UnitofTime); }
            set
            {
                progressindicator = value;
                OnPropertyChanged("ProgressIndicator");
            }
        }

         public string UnitofTime
        {
            get { return timeunit; }
            set
            {
                UnitofTime = value;
                OnPropertyChanged("ProgressIndicator");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
