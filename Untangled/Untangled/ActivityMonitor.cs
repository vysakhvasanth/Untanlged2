using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using CardControlLibrary;
using Path = System.IO.Path;

namespace Untangled
{
    /// <summary>
    /// Takes care of monitoring activites. Currently looks at deletion and creation of files
    /// in a specified folder.
    /// </summary>
    public class ActivityMonitor
    {
        private FileSystemWatcher _fMonitor;
        private readonly string FilePath;
        private readonly MainWindow mnWindow;
        private static object _syncLock = new object(); //lock object for synchronization
        private double Time;
        public event EventHandler StackMainEmpty;

        public ActivityMonitor(MainWindow mnWindow, string filePath, double Time)
        {
            this.mnWindow = mnWindow;
            FilePath = filePath;
            this.Time = Time;
        }

        /// <summary>
        /// Initiates a Monitor on certain folder
        /// </summary>
        public void StartMonitor()
        {
            _fMonitor = new FileSystemWatcher
            {
                Path = FilePath,
                Filter = "*.*",
                NotifyFilter = NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.FileName |
                               NotifyFilters.DirectoryName,
                //IncludeSubdirectories = true
            };

            _fMonitor.Deleted += _fMonitor_Deleted;
            _fMonitor.Created += _fMonitor_Created;
            _fMonitor.Renamed += _fMonitor_Renamed;
            _fMonitor.EnableRaisingEvents = true;

        }

        /// <summary>
        /// Kills an existing instance of the ActivityMonitor class
        /// Called when required to stop monitoring a folder
        /// </summary>
        public void KillMonitor()
        {
            if (_fMonitor!=null)
            {
                _fMonitor.Deleted -= _fMonitor_Deleted;
                _fMonitor.Created -= _fMonitor_Created;
                _fMonitor.EnableRaisingEvents = false;
                _fMonitor = null; 
            }
        }

        private void _fMonitor_Renamed(object sender, RenamedEventArgs e)
        {
            mnWindow.StackMain.Dispatcher.Invoke(
                () =>
                    RenameCard(e.OldFullPath, e.FullPath,
                        e.ChangeType.ToString()));

            Logger(e.FullPath,Path.GetFileName(e.FullPath), string.Format("{0} ({1})",e.ChangeType.ToString(),e.OldName));
        }

        private void _fMonitor_Created(object sender, FileSystemEventArgs e)
        {
            mnWindow.Dispatcher.Invoke((() => Time = mnWindow.GeneralSettings.ClockTime)); //update timer to the current value
            var bg = new BackgroundWorker();  //BackgroundThread for adding the cards
            bg.DoWork += (i, j) => mnWindow.StackMain.Dispatcher.Invoke(new Action(() => CreateCard(e.Name,e.FullPath,e.ChangeType.ToString())));
            bg.RunWorkerAsync();

            Logger(e.FullPath, Path.GetFileName(e.FullPath), e.ChangeType.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fMonitor_Deleted(object sender, FileSystemEventArgs e)
        {
            mnWindow.StackMain.Dispatcher.Invoke(() => DeleteCardItem(e.FullPath));
            Logger(e.FullPath, Path.GetFileName(e.FullPath), e.ChangeType.ToString());
        }

        /// <summary>
        /// Adds the card element to grid
        /// </summary>
        /// <param name="item"> Instance of a FileDetails class </param>
        public void CreateCard( string name, string fullpath, string activity)
        {
            var cardProperties = new CardProperties()
            {
                Name = name,
                Path = fullpath,
                Activity = activity,
                UnitofTime = "S",
            };
            
            switch (ToolBox.DirectoryOrFile(fullpath))
            {
                case FileType.Directory:
                    cardProperties.FileType = FileType.Directory;
                    cardProperties.FileTypeLabel = "Directory:  ";
                    cardProperties.IconImage = cardProperties.TypeOfFile[1];
                    break;
                case FileType.File:
                    cardProperties.FileType = FileType.File;
                    cardProperties.FileTypeLabel = "File:  ";
                    cardProperties.IconImage = cardProperties.TypeOfFile[0];
                    break;
            }
         
            var awesomeCard = new AwesomeCard
            {
                DataContext = cardProperties,
                CardProperties = cardProperties,
                _assocFilePath = cardProperties.Path,
                _filename =  Path.GetFileName(cardProperties.Path),
                Width = 490,
                Height = 150
            };

            // register to selected item dictionary for changes
            ToolBox.SelectedCardCollection.isDictEmpty += SelectedCardCollection_isDictEmpty;

            //Time = 10;
            awesomeCard.TimerFinished += awesomeCardTimer_Finished;
            awesomeCard.Selected += awesomeCard_Selected;
            awesomeCard.StartTimer(Time);
            ToolBox.CardCollection.Add(awesomeCard._filename, awesomeCard);

            mnWindow.TxtDefault.Visibility = Visibility.Hidden; //Update state
            int elementId = mnWindow.StackMain.Children.Add(awesomeCard);
            mnWindow.GeneralSettings.ItemsCount++;
            awesomeCard._elementId = elementId;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteCardItem(string fileName)
        {
            if (!ToolBox.CardCollection.ContainsKey(Path.GetFileName(fileName)))
                return;

            var currentCard = ToolBox.CardCollection[Path.GetFileName(fileName)];//Stop the existing timer running on this file, or it will run
            currentCard.Timer.Stop();                                            //and delete things in the background

            mnWindow.StackMain.Children.Remove(currentCard);//Update application state
            mnWindow.GeneralSettings.ItemsCount--;

            if(mnWindow.StackMain.Children.Count <= 0) //raise an event when stackpanel is empty
                StackMainEmpty(this,new EventArgs());

            ToolBox.CardCollection.Remove(Path.GetFileName(fileName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldfileName"></param>
        /// <param name="newfileName"></param>
        /// <param name="activity"></param>
        public void RenameCard(string oldfileName, string newfileName, string activity)
        {
            if (!ToolBox.CardCollection.ContainsKey(Path.GetFileName(oldfileName)))
                return;

            var currentCard = ToolBox.CardCollection[Path.GetFileName(oldfileName)];
            currentCard.CardProperties.Name = Path.GetFileName(newfileName);
            currentCard.CardProperties.Path = newfileName;
            currentCard.CardProperties.Activity = activity;
            currentCard._assocFilePath = newfileName;
            currentCard._filename = Path.GetFileName(newfileName);

            ToolBox.CardCollection.Remove(Path.GetFileName(oldfileName)); //remove old key and add new key with updated item
            ToolBox.CardCollection.Add(Path.GetFileName(newfileName), currentCard);
        }

        void awesomeCardTimer_Finished(object sender, EventArgs e)
        {
            var card = sender as AwesomeCard;
            if (card == null) return;
            try
            {
                new DeletionMechanism(card._assocFilePath, card.CardProperties.FileType);
            }
            catch (Exception ex)
            {
                mnWindow.DisplayAwesomeMessage(mnWindow,ex.Message,MessageType.Critical, 0);
            }
        }

        void awesomeCard_Selected(object sender, EventArgs e)
        {
            var card = sender as AwesomeCard;
            if(card == null) return;

            if (card.CurrentState == State.NormalSelected)
            {
                HandleSelection(card);
            }
            else
            {
                HandleUnSelection(card);
            }
        }

        void SelectedCardCollection_isDictEmpty(object sender, EventArgs e)
        {
            mnWindow.BtnSettings.IsEnabled = true;
            mnWindow.BtnRules.IsEnabled = true;
            mnWindow.MultiFunctions.Visibility = Visibility.Hidden;
            mnWindow.Btnselect.Visibility = Visibility.Visible;
            mnWindow.BtnUnselect.Visibility = Visibility.Collapsed;
        }

        public void HandleSelection(AwesomeCard card)
        {
            card.CardProperties.IconImage = card.CardProperties.TypeOfFile[2];
            if (!ToolBox.SelectedCardCollection.ContainsKey(card._filename))
                ToolBox.SelectedCardCollection.Add(card._filename, card);

            if (ToolBox.SelectedCardCollection.Count > 0)
            {
                mnWindow.BtnSettings.IsEnabled = false;
                mnWindow.BtnRules.IsEnabled = false;
                mnWindow.MultiFunctions.Visibility = Visibility.Visible;
            }
            
        }


        public void HandleUnSelection(AwesomeCard card)
        {
          
            ToolBox.SelectedCardCollection.Remove(card._filename);
            if (ToolBox.SelectedCardCollection.Count == 0)
            {
                mnWindow.BtnSettings.IsEnabled = true;
                mnWindow.BtnRules.IsEnabled = true;
                mnWindow.MultiFunctions.Visibility = Visibility.Hidden;
            }


            switch (card.CardProperties.FileType)
            {
                case FileType.Directory:
                    card.CardProperties.IconImage = card.CardProperties.TypeOfFile[1];
                    break;
                case FileType.File:
                    card.CardProperties.IconImage = card.CardProperties.TypeOfFile[0];
                    break;
            }
        }
        /// <summary>
        /// Simple data logger
        /// </summary>
        private void Logger(string filetype, string path, string activity)
        {

            var logstring = new StringBuilder("");

            mnWindow.LoggerBox.Dispatcher.Invoke(
                () =>

                    mnWindow.LoggerBox.AppendText(string.Format("{0}\tName: {1}\n\t\tPath: {2}\n\t\tActivity: {3}\n\n",DateTime.Now.ToString("hh:mm:ss.fff"),Path.GetFileName(path), filetype
                        ,activity)));
        }
    }
}
