using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CardControlLibrary;
using LearningWPF.Annotations;

namespace LearningWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    
    public partial class MainWindow : Window
    {
        private Person person;
        //private FileInfo info;
        public MainWindow()
        {
            InitializeComponent();
            person = new Person(){Name ="Vas", Age=22};
            this.DataContext = person;

            var info = new FileInfo()
            {
                Name = "test",
                Path = @"C:\test.xaml",
                Activity = "Created",
                FileType = FileType.File,
                Progress = 100
            };

            card1.DataContext = info;
            card1._fileInfo = info;
            card2.DataContext = info;
            card2._fileInfo = info;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            card1._fileInfo.Name = "uid.xaml";
            card1._fileInfo.Path = @"C:\uid.xaml"; 
            card1._fileInfo.Activity = "Renamed";
            card1._fileInfo.Progress-=10;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("{0}", card1._fileInfo.Progress), "Progress!");
        }

    }

    public class Person: INotifyPropertyChanged
    {
        private string nameVal;

        public string Name
        {
            get { return nameVal; }
            set { nameVal = value; }
        }

        private int ageVal;
        public int Age
        {
            get { return ageVal; }
            set {
                if (value == ageVal) return;
                ageVal = value;
                OnPropertyChanged("Age");
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
