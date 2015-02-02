using System.ComponentModel;

namespace Untangled
{
    public class GenericSettings : INotifyPropertyChanged
    {
        private string _monitoringdirectory;
        private double _clocktime;
        private int _itemsCount;
        private string _itemsIndicator;

        public string MonitoringDirectory
        {
            get { return _monitoringdirectory; }
            set
            {
                if (value == _monitoringdirectory) return;
                _monitoringdirectory = value;
                OnPropertyChanged("MonitoringDirectory");
            }
        }

        public double ClockTime
        {
            get { return _clocktime; }
            set
            {
                if (value == _clocktime) return;
                _clocktime = value;
                OnPropertyChanged("ClockTime");
            }
        }

        public int ItemsCount
        {
            get { return _itemsCount; }
            set
            {
                if (value == _itemsCount) return;
                if (value == 1)
                {
                    _itemsIndicator = value + " item";
                }
                else
                {
                    _itemsIndicator = value + " items";
                }
                _itemsCount = value;
                OnPropertyChanged("ItemsCount");
                OnPropertyChanged("ItemsIndicator");
            }
        }

        public string ItemsIndicator
        {
            get { return _itemsIndicator; }
            set
            {
                if (value == _itemsIndicator) return;
                _itemsIndicator = value;
                OnPropertyChanged("ItemsIndicator");
            }
        }

        public GenericSettings()
        {
            ItemsIndicator = "0 items";
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate {};

        private void OnPropertyChanged(string propertyName)
        {
           PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
