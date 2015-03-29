using System.ComponentModel;
using System.Windows.Media;

namespace CardControlLibrary
{
    public class CardProperties : INotifyPropertyChanged
    {
        private string name;
        private string path;
        private string activity;
        private double progress;
        private string progressindicator;
        private string unitoftime;
        public string iconImage;
        private FileType filetype;
        private string filetypelabel;
        public double Time { get;set; }
        public  string[] TypeOfFile = { "pack://application:,,,/CardControlLibrary;component/Icons/fileicon.png", "pack://application:,,,/CardControlLibrary;component/Icons/directoryicon.png", 
                                                "pack://application:,,,/CardControlLibrary;component/Icons/tickicon.png" };

        public CardProperties()
        {
            //setting default property values
            strokeBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#70E078"));
            filetypelabel = "File:  ";
            unitoftime = "M";
            ellipseFill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3EC97C"));
            IconImage = TypeOfFile[0];
            progress = 100.0;
        }

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

        public string FileTypeLabel
        {
            get { return filetypelabel; }
            set
            {
                if (value == filetypelabel) return;
                filetypelabel = value;
                OnPropertyChanged("FileTypeLabel");
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
        public double Progress
        {
            get { return progress; }
            set
            {
                if (value == progress) return;
                if (value < 0) return;
                
                progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public string ProgressIndicator
        {
            get { return string.Format("{0} {1}", progressindicator, UnitofTime); }
            set
            {
                progressindicator = value;
                OnPropertyChanged("ProgressIndicator");
            }
        }

        public string UnitofTime
        {
            get { return unitoftime; }
            set
            {
                unitoftime = value;
                OnPropertyChanged("ProgressIndicator");
            }
        }

        private Brush strokeBrush;
        public Brush StrokeBrush
        {
            get { return strokeBrush; }
            set
            {
                strokeBrush = value;
                OnPropertyChanged("StrokeBrush");

            }
        }

        private Brush ellipseFill;
        public Brush EllipseFill
        {
            get { return ellipseFill; }
            set
            {
                ellipseFill = value;
                OnPropertyChanged("EllipseFill");

            }
        }

        public string IconImage
        {
            get { return iconImage; }
            set
            {
                iconImage = value;
                OnPropertyChanged("IconImage");
            }
        }

        public void UpdateProgresBar(double val)
        {
            if(Progress <= 0) return;
            Progress = Progress - (Progress - val);

            if (Progress <= 50.0 && Progress > 20.0)
            {
                StrokeBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6AA5C"));
                EllipseFill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE922F"));
            }
            else if (Progress <= 20.0)
            {
                EllipseFill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C41A55"));
                StrokeBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE5082"));
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
