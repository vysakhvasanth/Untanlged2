using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using CardControlLibrary;

namespace CardControlLibrary
{
    public class AwesomeCard : ContentControl
    {
        public long _unqId { get; set; }
        public string _assocFilePath { get; set; }
        public FileInfo _fileInfo { get; set; }
        public DispatcherTimer timer { get; set; }

        static AwesomeCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (AwesomeCard),
                new FrameworkPropertyMetadata(typeof (AwesomeCard)));
        }


    }
}
