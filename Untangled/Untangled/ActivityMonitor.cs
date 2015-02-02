using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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

            Logger("",Path.GetFileName(e.FullPath), e.ChangeType.ToString());
        }

        private void _fMonitor_Created(object sender, FileSystemEventArgs e)
        {
            mnWindow.Dispatcher.Invoke((() => Time = mnWindow.GeneralSettings.ClockTime)); //update timer to the current value
            var bg = new BackgroundWorker();  //BackgroundThread for adding the cards
            bg.DoWork += (i, j) => mnWindow.StackMain.Dispatcher.Invoke(new Action(() => CreateCard(e.Name,e.FullPath,e.ChangeType.ToString())));
            bg.RunWorkerAsync();

            Logger("", Path.GetFileName(e.FullPath), e.ChangeType.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fMonitor_Deleted(object sender, FileSystemEventArgs e)
        {
            mnWindow.StackMain.Dispatcher.Invoke(() => DeleteCardItem(e.FullPath));
            Logger("", Path.GetFileName(e.FullPath), e.ChangeType.ToString());
        }

        /// <summary>
        /// Adds the card element to grid
        /// </summary>
        /// <param name="item"> Instance of a FileDetails class </param>
        public void CreateCard( string name, string fullpath, string activity)
        {
            var fileTypeLabel = "File:  ";
            var fileType = FileType.Unknown;
            var iconstring = "";
            switch (ToolBox.DirectoryOrFile(fullpath))
            {
                case FileType.Directory:
                    fileType = FileType.Directory;
                    fileTypeLabel = "Directory:  ";
                    iconstring = CardProperties.TypeOfFile[1];
                    break;
                case FileType.File:
                    fileType = FileType.File;
                    iconstring = CardProperties.TypeOfFile[0];
                    break;
            }

            var cardProperties = new CardProperties()
            {
                Name = name,
                Path = fullpath,
                Activity = activity,
                FileType = fileType,
                FileTypeLabel =  fileTypeLabel,
                IconImage = iconstring,
                UnitofTime = "S",
            };
            var awesomeCard = new AwesomeCard
            {
                DataContext = cardProperties,
                CardProperties = cardProperties,
                _assocFilePath = cardProperties.Path,
                Width = 490,
                Height = 150
            };

            //Time = 10;
            awesomeCard.TimerFinished += awesomeCardTimer_Finished;
            awesomeCard.StartTimer(Time);
            ToolBox.CardCollection.Add(Path.GetFileName(awesomeCard._assocFilePath), awesomeCard);

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
            currentCard.timer.Stop();                          //and delete things in the background

            mnWindow.StackMain.Children.Remove(currentCard);//Update application state
            mnWindow.GeneralSettings.ItemsCount--;
            mnWindow.HandleEmptyDirectoryNotification();
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
                //
            }
        }

        /// <summary>
        /// Simple data logger
        /// </summary>
        private void Logger(string filetype, string path, string activity)
        {
            mnWindow.LoggerBox.Dispatcher.Invoke(
                () =>

                    mnWindow.LoggerBox.AppendText(string.Format(
                        "{0}: {1} \nActivity: {2}\n---------------------------", filetype,
                        Path.GetFileName(path), activity)));

        }

    }
}
