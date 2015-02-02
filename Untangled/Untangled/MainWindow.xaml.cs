using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CardControlLibrary;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
namespace Untangled
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static string Configfile = "sweetloader.xml";
        private string _applicationDirectory;
        private string _watchFolder;
        private NotifyIcon _notifyIcon;
        private ActivityMonitor _acm;
        private Application _applicationInstance;

        public GenericSettings GeneralSettings
        {
            get { return (GenericSettings)GetValue(GeneralSettingsProperty); }
            set { SetValue(GeneralSettingsProperty, value); }
        }

        //Using a DependencyProperty as the backing store for GeneralSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeneralSettingsProperty =
            DependencyProperty.Register("GeneralSettings", typeof(GenericSettings), typeof(MainWindow), new PropertyMetadata(null));


        public MainWindow()
        {
            InitializeComponent();
            FundamentalSetup();
            _applicationDirectory = Directory.GetCurrentDirectory(); //get current directory
            LoadtoTray();
            LoadConfiguration(Path.Combine(_applicationDirectory, Configfile));
            this.DataContext = GeneralSettings;
        }

        /// <summary>
        /// Set some application specific settings
        /// </summary>
        private void FundamentalSetup()
        {
            _applicationInstance = Application.Current;
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),  //lower frame rate to reduce CPU usage, best trade off is val 30. (I think!)
                new FrameworkPropertyMetadata { DefaultValue = 30 });

        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadtoTray()
        {
            //Initialize NotifyIcon instance,
            // Hides the window to tray,
            // Note: This will fail if there is no icon
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
            };

            try
            {
                _notifyIcon.Icon = new Icon("icon.ico");
            }
            catch (Exception ex)
            {
                DisplayAwesomeMessage(this,ex.Message+ex.StackTrace+ex.TargetSite.ToString(),MessageType.Critical);
            }

            if (_notifyIcon != null) //display form on icon click
                _notifyIcon.Click += delegate
                {
                    Show();
                    Activate(); //bring window to front
                    WindowState = WindowState.Normal;
                };

            //hide to tray on launch
            Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadConfiguration(string filePath)
        {
            //If path invalid, create a new configuration file prompt user to configure a folder
            if (!File.Exists(filePath))
            {
                try
                {
                    File.Create(filePath).Close(); //close is required since it opens a filestream on the file
                    var emptyConfiguration = new ConfigurationLoader //write an empty xml header, so you dont get xml errors on first bootup
                    {
                        FolderPath = "",
                        ClockTime =  120
                    };
                    ToolBox.Serialize(emptyConfiguration,filePath);
                    HandleDirectoryNotSet(true);
                }
                catch (Exception ex)
                {
                    DisplayAwesomeMessage(this,ex.Message,MessageType.Critical);
                }
            }
            else
            {
                //try load from the file and do validation
                try
                {
                    var savedConfiguration = ToolBox.Deserialize(Configfile);
                    if (string.IsNullOrEmpty(savedConfiguration.FolderPath))
                    {
                        Show();
                        DisplayAwesomeMessage(this, "No directory path set in configuration file. Set one lazy!", MessageType.Warning);
                        HandleDirectoryNotSet(false);
                    }
                    else
                    {
                        _watchFolder = savedConfiguration.FolderPath;
                        GeneralSettings = new GenericSettings()
                        {
                            ClockTime = savedConfiguration.ClockTime,
                            MonitoringDirectory = savedConfiguration.FolderPath
                        };
                        SettingsViewScroller.DataContext = GeneralSettings; //sets datacontext of all controls in the mainform
                        DirectoryName.IsEnabled = false; //only enable once you click folder choose button
                        CommenceMonitoring(_watchFolder);
                    }
                }
                catch (Exception ex)
                {
                   DisplayAwesomeMessage(this,ex.Message, MessageType.Critical);
                   HandleDirectoryNotSet(true);
                }
            }

            if (GeneralSettings == null) //initialise an instance of GeneralSettings to avoid null exception
            {
                GeneralSettings = new GenericSettings() 
                {
                    MonitoringDirectory = "",
                    ClockTime = 120
                };
            }

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        private void CommenceMonitoring(string folder)
        {
            //kill exisiting instance to avoid duplicate watchers
            //TODO: allow multiple seperate watchers to monitor different folders simutaneously
            if (_acm != null)
            {
                _acm.KillMonitor();
            }
            TxtDefault.Text = "You are watching" + Environment.NewLine + "\"" + Path.GetFileName(_watchFolder) + "\"";
            var acm = new ActivityMonitor(this, folder,GeneralSettings.ClockTime);
            _acm = acm;
            acm.StartMonitor();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewState"></param>
        private void SetViewState(ViewState viewState)
        {
            switch (viewState)
            {
                case ViewState.Main:
                    DirectoryName.IsEnabled = false; //only enable once you click folder choose button
                    MainScroller.Visibility = Visibility.Visible;
                    SettingsViewScroller.Visibility = Visibility.Hidden;
                    LogViewerScroller.Visibility = Visibility.Hidden;
                    BtnBack.Visibility = Visibility.Hidden;
                    TxtDefault.Visibility = StackMain.Children.Count != 0 ? Visibility.Hidden : Visibility.Visible;
                    SearchBoxLabel.Visibility = SearchBox.Visibility = ItemsIndicator.Visibility = Visibility.Visible;
                    break;
                case ViewState.Settings:
                    SettingsViewScroller.Visibility = Visibility.Visible;
                    MainScroller.Visibility = Visibility.Hidden;
                    BtnBack.Visibility = Visibility.Visible;
                    LogViewerScroller.Visibility = Visibility.Hidden;
                    TxtDefault.Visibility = Visibility.Hidden;
                    SearchBoxLabel.Visibility = SearchBox.Visibility = ItemsIndicator.Visibility = Visibility.Collapsed;
                    break;
                case ViewState.Log:
                    SettingsViewScroller.Visibility = Visibility.Hidden;
                    MainScroller.Visibility = Visibility.Hidden;
                    BtnBack.Visibility = Visibility.Visible;
                    LogViewerScroller.Visibility = Visibility.Visible;
                    TxtDefault.Visibility = Visibility.Hidden;
                    SearchBoxLabel.Visibility = SearchBox.Visibility = ItemsIndicator.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleDirectoryNotSet(bool isShow)
        {
            SetViewState(ViewState.Settings);
            if (isShow)Show();
            WindowState = WindowState.Normal;
            DisplayAwesomeMessage(this, "You have to set a folder to watch!", MessageType.Warning);
            TxtDefault.Text = ToolBox.Nofolderset;
        }

        /// <summary>
        /// 
        /// </summary>
        public void HandleEmptyDirectoryNotification()
        {
            if (MainScroller.IsVisible && StackMain.Children.Count == 0)
            {
                TxtDefault.Visibility = Visibility.Visible;
            }
        }

        public void DisplayAwesomeMessage(Window owner, string text, MessageType messageType)
        {
            new ElegantMessageBox(owner, text, messageType);
        }


        #region All Event Handlers
        private void UntangledWnd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == Mouse.LeftButton)
            {
              // DragMove(); //enables a borderless window to be dragged
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

        //custom close button event
        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close(); 
        }

        //custom minimize button event
        private void _btnMinimise_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        //display settings grid on the stack panel
        //display back button on the sidebar (navigation)
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
           SetViewState(ViewState.Settings);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
           SetViewState(ViewState.Main);
        }

        //set location of window to bottom right corner, stackoverflow question: 7620488
        private void UntangledWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }

        //remove notifyicon from tray once closed
        private void UntangledWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _notifyIcon.Visible = false;
        }

        
        private void BtnFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            DirectoryName.IsEnabled = true;
            var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();

            try
            {
                //if user canceled, set it back to exisiting one
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    GeneralSettings.MonitoringDirectory = _watchFolder;
                }
                else
                {
                    _watchFolder = GeneralSettings.MonitoringDirectory = fbd.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                DisplayAwesomeMessage(this, ex.Message+ex.StackTrace.ToString(), MessageType.Critical);
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //save configurations to xml
            //and reload
            DirectoryName.IsEnabled = false; //only enable once you click folder choose button
            if (string.IsNullOrEmpty(DirectoryName.Text) || !Directory.Exists(DirectoryName.Text))
            {
                DisplayAwesomeMessage(this, "You have to set a valid path!", MessageType.Warning);
            }
            else
            {
                _watchFolder = GeneralSettings.MonitoringDirectory;
                var cfl = new ConfigurationLoader
                {
                    FolderPath = GeneralSettings.MonitoringDirectory,
                    ClockTime = GeneralSettings.ClockTime
                };

                //serialise to xml file
                ToolBox.Serialize(cfl, Configfile);
                DisplayAwesomeMessage(this, "Settings saved!", MessageType.Notify);
                CommenceMonitoring(_watchFolder);
            }
        }

        private void BtnRules_Click(object sender, RoutedEventArgs e)
        {
            SetViewState(ViewState.Log);
        }

        private void UntangledWindow_Deactivated(object sender, EventArgs e)
        {
            ;
        }

        private void BtnCancelSettings_Click(object sender, RoutedEventArgs e)
        {
            SetViewState(ViewState.Main);
        }

        private void BtnPinClick(object sender, RoutedEventArgs e)
        {
            _applicationInstance.Deactivated -= app_Deactivated;
            BtnNoPin.Visibility = Visibility.Visible;
            BtnPin.Visibility = Visibility.Hidden;
        }

        private void BtnNoPinClick(object sender, RoutedEventArgs e)
        {
            _applicationInstance.Deactivated += app_Deactivated;

            //update button states
            BtnPin.Visibility = Visibility.Visible;
            BtnNoPin.Visibility = Visibility.Hidden;
        }

        private void app_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void UntangledWindow_Activated(object sender, EventArgs e)
        {
            HandleEmptyDirectoryNotification();
        }
        #endregion

        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SearchBox.Clear();
                GeneralSettings.ItemsCount = StackMain.Children.Count;
                FilterCards(new Regex(".*"));
                return;
            }

            if (e.Key != Key.Return || StackMain.Children.Count == 0 || string.IsNullOrEmpty(SearchBox.Text)) return;
            DisplayAwesomeMessage(this, "Showing search results...",MessageType.Notify);

            try
            {
                var regex = new Regex(SearchBox.Text,RegexOptions.IgnoreCase);
                FilterCards(regex);
            }
            catch (Exception ex)
            {
                DisplayAwesomeMessage(this, "Invalid regular expression", MessageType.Warning);
            }

        }

        private void FilterCards(Regex regex)
        {
            int results = 0;

            for (int i = 0; i < StackMain.Children.Count; i++)
            {
                var card = StackMain.Children[i] as AwesomeCard;
                if (card == null) continue;

                if (!regex.IsMatch(card.CardProperties.Name))
                {
                    card.Visibility = Visibility.Collapsed;
                }
                else
                {
                    card.Visibility = Visibility.Visible;
                    GeneralSettings.ItemsCount = ++results;
                }

                if (results == 0)
                    GeneralSettings.ItemsCount = results;
            }
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                SearchBox.Clear();
                GeneralSettings.ItemsCount = StackMain.Children.Count;
                FilterCards(new Regex(".*"));
            }
        }

        #region Redundant code

        //private bool _scroll = true;
        //private void MainScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    //if (e.ExtentHeight == 0)
        //    //{
        //    //    if (MainScroller.VerticalOffset == MainScroller.ScrollableHeight)
        //    //    {
        //    //        _scroll = true;
        //    //    }
        //    //    else
        //    //    {
        //    //        _scroll = false;
        //    //    }
        //    //}

        //    //if (_scroll && e.ExtentHeight != 0)
        //    //{
        //    //    MainScroller.ScrollToVerticalOffset(MainScroller.ExtentHeight);
        //    //}
        //}

        #endregion

    }
}
