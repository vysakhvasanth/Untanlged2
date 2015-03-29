using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using CardControlLibrary;
using MessageBox = System.Windows.MessageBox;

namespace Untangled
{
    /// <summary>
    /// A couple of static functions
    /// </summary>

    public class ToolBox
    {

        public static string Nofolderset = "Point me to a folder human!";
        public static string Nofilestowatch = "Waiting for omnoms!";
        public static readonly Dictionary<string, AwesomeCard> CardCollection = new Dictionary<string, AwesomeCard>();

        public static ObservableDict<string, AwesomeCard> SelectedCardCollection = new ObservableDict<string, AwesomeCard>();

        public static void Serialize(ConfigurationLoader configuration, string fileName)
        {
            var ser = new XmlSerializer(typeof (ConfigurationLoader));
            using (var writer = new StreamWriter(fileName))
            {
                ser.Serialize(writer, configuration);
            }
        }

        public static ConfigurationLoader Deserialize(string fileName)
        {
            var deser = new XmlSerializer(typeof (ConfigurationLoader));
            object obj;
            using (var reader = new StreamReader(fileName))
            {
                obj = deser.Deserialize(reader);
            }

            if (obj != null) return (ConfigurationLoader) obj;

            return new ConfigurationLoader();
        }


        public static FileType DirectoryOrFile(string path)
        {
            if (Directory.Exists(path)) return FileType.Directory;
            return File.Exists(path) ? FileType.File : FileType.Unknown;
        }

    }

    public enum ViewState
    {
        Main,
        Settings,
        Log
    }

}
    
