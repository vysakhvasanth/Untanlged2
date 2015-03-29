using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Untangled
{
    /// <summary>
    /// Interaction logic for ElegantMessageBox.xaml
    /// </summary>
    public partial class ElegantMessageBox : Window
    {
        private Window owner;
        private MessageType messageType;
        private DispatcherTimer timer;
        private int Time;

        public ElegantMessageBox(Window owner, string text, MessageType messageType, int Time)
        { 
            InitializeComponent();
            this.owner = owner;
            this.Time = Time;
            this.messageType = messageType;
            DataContext = new Data() { Message = text };
            SetWindowProperties();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - (Width + ((owner.Width/2) - (Width/2)) - 40);
            Top = desktopWorkingArea.Bottom - (Height + 80);
        }

        public void SetWindowProperties()
        {
            switch (messageType)
            {
                case MessageType.Notify:
                    timer = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0, 0, 0, 0, Time)
                    };
                    timer.Tick += timer_Tick;
                    timer.Start();

                    Wrapper.Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#83C730"));
                    Wrapper.Opacity = 0.9;
                    Wrapper.BorderThickness = new Thickness(1);
                    Wrapper.BorderBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#5C9E0B"));
                    MessageTxt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                    Show();
                    break;
                case MessageType.Warning:
                    timer = new DispatcherTimer()
                    {
                        Interval = new TimeSpan(0,0,0,0,Time)
                    };
                    timer.Tick += timer_Tick;
                    timer.Start();

                    Wrapper.Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#F7B843"));
                    Wrapper.Opacity = 0.9;
                    Wrapper.BorderThickness = new Thickness(1);
                    Wrapper.BorderBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#C28617"));
                    MessageTxt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E"));
                    Show();
                    break;
                case MessageType.Critical:
                    Wrapper.Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#E02B2B"));
                    Wrapper.Opacity = 0.9;
                    Wrapper.BorderThickness = new Thickness(1);
                    Wrapper.BorderBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#B01515"));
                    MessageTxt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                    ShowDialog();
                    break;

            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Close();
            timer.Stop();
        }

        private void Wrapper_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (messageType)
            {
                case MessageType.Notify:
                    Close();
                    break;
                case MessageType.Critical:
                    Close();
                    break;
                case MessageType.Warning:
                    Close();
                    break;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            switch (messageType)
            {
                case MessageType.Notify:
                    Close();
                    break;
                case MessageType.Critical:
                    break;
                case MessageType.Warning:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //fade out animation code
            Closing -= Window_Closing;
            e.Cancel = true;
            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            fadeOutAnimation.Completed += (s, a) => Close();
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }
    }

    public class Data : INotifyPropertyChanged
    {
        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
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

    public enum MessageType
    {
        Notify,
        Warning,
        Critical
    }
}
