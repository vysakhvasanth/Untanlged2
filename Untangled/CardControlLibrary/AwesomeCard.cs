using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CardControlLibrary
{
    public class AwesomeCard : ContentControl
    {
        public Guid _unqId;
        public int _elementId;
        public string _assocFilePath { get; set; }
        public string _filename { get; set; }
        public CardProperties CardProperties { get; set; }
        public DispatcherTimer Timer { get; set; }
        public State CurrentState { get; set; }
        public Disposible CurrentDisposibleState { get; set; }
        public FileType _fileType { get; set; }
        public event EventHandler TimerFinished;
        public event EventHandler Selected;
        public ControlState controlState = ControlState.Pause;
       


        private Border theBorder;
        protected Border TheBorder
        {
            get { return theBorder; }
            set
            {
                if (theBorder != null)
                {
                    
                }

                theBorder = value;

                if (theBorder != null)
                {
                   
                }
            }
        }

        private Image imageIcon;
        protected Image ImageIcon
        {
            get { return imageIcon; }
            set
            {
                if (imageIcon != null)
                {
                    imageIcon.MouseLeftButtonDown -= imageIcon_MouseLeftButtonDown;
                    imageIcon.MouseLeftButtonUp -= imageIcon_MouseLeftButtonUp;
                }

                imageIcon = value;

                if (imageIcon != null)
                {
                    imageIcon.MouseLeftButtonDown += imageIcon_MouseLeftButtonDown;
                    imageIcon.MouseLeftButtonUp += imageIcon_MouseLeftButtonUp;
                }
            }
        }


        private Grid fullGrid;
        protected Grid FullGrid
        {
            get { return fullGrid; }
            set
            {
                if (fullGrid != null)
                {
                    fullGrid.MouseEnter -= fullGrid_MouseEnter;
                    fullGrid.MouseLeave -= fullGrid_MouseLeave;
                }

                fullGrid = value;

                if (fullGrid != null)
                {
                    fullGrid.MouseEnter += fullGrid_MouseEnter;
                    fullGrid.MouseLeave += fullGrid_MouseLeave;
                }
            }
        }

        private TextBlock progressIndicator;
        protected TextBlock ProgressIndicator
        {
            get { return progressIndicator; }
            set { progressIndicator = value; }
        }

        private Button pauseButton;
        public Button PauseButton
        {
            get { return pauseButton; }
            set
            {
                if (pauseButton != null)
                    pauseButton.Click -= pauseButton_Click;

                pauseButton = value;

                if (pauseButton!=null)
                    pauseButton.Click +=pauseButton_Click;

            }
        }

        private Button playButton;
        public Button PlayButton
        {
            get { return playButton; }
            set
            {
                if (playButton != null)
                    playButton.Click -= playButton_Click;

                playButton = value;

                if (playButton != null)
                    playButton.Click += playButton_Click;

            }
        }

        private AwesomeProgressBar awesomebar;
        protected AwesomeProgressBar AwesomeProgress
        {
            get { return awesomebar; }
            set
            {
                if (awesomebar != null) ;
                awesomebar = value;
                if (awesomebar != null) ;
            }
        }

        private Ellipse ellipse;
        protected Ellipse Ellipse
        {
            get { return ellipse; }
            set { ellipse = value; }
        }

        static AwesomeCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (AwesomeCard),
                new FrameworkPropertyMetadata(typeof (AwesomeCard)));
        }

        public AwesomeCard()
        {
            CurrentState = State.Normal;
            CurrentDisposibleState = Disposible.NoMan;
            _unqId = Guid.NewGuid();
            _fileType = FileType.File;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // fetching the controls form the template
            // TODO: this is not a good MVVM practice, improve it with view-model
            AwesomeProgress = GetTemplateChild("AwesomeBar") as AwesomeProgressBar;
            PauseButton = GetTemplateChild("PauseButton") as Button;
            PlayButton = GetTemplateChild("PlayButton") as Button;
            ProgressIndicator = GetTemplateChild("ProgressValue") as TextBlock;
            Ellipse = GetTemplateChild("TimerEllipse") as Ellipse;
            FullGrid = GetTemplateChild("FullProgress") as Grid;
            TheBorder = GetTemplateChild("TheBorder") as Border;
            ImageIcon = GetTemplateChild("ImageIcon") as Image;
        }

        void fullGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            switch (controlState) //switch on controls and show them based on the state
            {
                case ControlState.Play:
                    PauseButton.Visibility = Visibility.Hidden;
                    PlayButton.Visibility = Visibility.Visible;
                    ProgressIndicator.Visibility = Visibility.Hidden;
                    break;
                case ControlState.Pause:
                    PauseButton.Visibility = Visibility.Visible;
                    PlayButton.Visibility = Visibility.Hidden;
                    ProgressIndicator.Visibility = Visibility.Hidden;
                    break;
            }
        }

        void fullGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            switch (controlState) //switch on controls and show them based on the state
            {
                case ControlState.Play:
                    ProgressIndicator.Visibility = Visibility.Hidden;
                    PauseButton.Visibility = Visibility.Hidden;
                    PlayButton.Visibility = Visibility.Visible;
                    break;
                case ControlState.Pause:
                    ProgressIndicator.Visibility = Visibility.Visible;
                    PauseButton.Visibility = Visibility.Hidden;
                    PlayButton.Visibility = Visibility.Hidden;
                    break;
            }
        }

        void imageIcon_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CurrentState == State.Normal)
            {
                CurrentState = State.NormalSelected;
                Selected(this, new EventArgs());
            }
            else
            {
                CurrentState = State.Normal;
                Selected(this, new EventArgs());
            }
        }

        void imageIcon_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           //
        }

        void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            ProgressIndicator.Visibility = Visibility.Hidden;
            PauseButton.Visibility = Visibility.Collapsed;
            PlayButton.Visibility = Visibility.Visible;
            controlState = ControlState.Play;
        }


        void playButton_Click(object sender, RoutedEventArgs e)
        {
            Timer.Start();
            ProgressIndicator.Visibility = Visibility.Hidden;
            PlayButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
            controlState = ControlState.Pause;
        }

        private double CurrentTime;
        private double ElapsedTime;
        /// <summary>
        /// Creates a dispatcher timer with the specified secTime.
        /// </summary>
        /// <param name="secTime"> Time in seconds. Min value of 1, Max value of 999999.</param>
        public void StartTimer(double secTime)
        {
            if (secTime < 1.0 || secTime > 999999.0)
            {
                MessageBox.Show("Time value not valid. It should be between 1 and 999999", "Alert!");
                return;
            }

            ElapsedTime = CurrentTime = CardProperties.Time = secTime; //setting secTime parameters to default
            SetTimeUnit(secTime); //set and display secTime appropriately

            Timer = new DispatcherTimer(DispatcherPriority.Render); //setup and start timer
            Timer.Tick += timer_Tick;
            Timer.Interval = Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ElapsedTime -= 0.5; //decreasing by 0.5 since the timer ticks twice every second
            double ElapsedTimePercent = (ElapsedTime / CurrentTime) * CardProperties.Progress; //decrease progress by 1, when the elapsed time drops by 1% from current time 
            
            CardProperties.UpdateProgresBar(ElapsedTimePercent);
            CurrentTime-=0.5; //update to current secTime
            SetTimeUnit(ElapsedTime); //set and display secTime according to the correct format
            
            if (CardProperties.Progress <= 0.0 || CurrentTime < 0.0)
            {
                TimerFinished(this, new EventArgs());
                Timer.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        private void SetTimeUnit(double val)
        {
            double TimeInInteger = 0;
            if (val >= 60.0 && val < 3600.0)
            {
                TimeInInteger = val/60;
                CardProperties.UnitofTime = "M";
                CardProperties.ProgressIndicator = Math.Round(TimeInInteger, 1).ToString("0");

            }
            else if (val >= 3600.0 && val < 86400.0)
            {
                TimeInInteger = val/3600;
                CardProperties.UnitofTime = "H";
                CardProperties.ProgressIndicator = Math.Round(TimeInInteger, 1).ToString("0");

            }
            else if (val >= 86400.0)
            {
                TimeInInteger = val / 85400;
                CardProperties.UnitofTime = "D";
                CardProperties.ProgressIndicator = Math.Round(TimeInInteger, 1).ToString("0");
            }
            else
            {
                TimeInInteger = val;
                CardProperties.UnitofTime = "S";
                CardProperties.ProgressIndicator = Math.Round(TimeInInteger,1).ToString("0");
            }
        }
    }

    public enum FileType
    {
        File,
        Directory,
        Unknown
    }

    public enum State
    {
        Normal,
        NormalSelected,
        Filtered,
        FilteredSelected
    }

    public enum Disposible
    {
        NoMan,
        BeCareful,
        YesMaster
    }

    public enum ControlState
    {
        Play,
        Pause
    }

}
