using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using CardControlLibrary;

namespace Untangled
{
    /// <summary>
    /// Class to Delete files/Directory
    /// </summary>
    public class DeletionMechanism
    {

        public DeletionMechanism(string path, FileType fileType)
        {
            //sets enum based on directory or file
            switch (fileType)
            {
                case FileType.Directory:
                    DeleteDirectory(path);
                    break;
                case FileType.File:
                    DeleteFile(path);
                    break;
                case FileType.Unknown:
                    MessageBox.Show("Unknown filetype! Man, ain't this about a ...", "Fault");
                    break;
            }
        }

        public void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fault");
            }
        }

        public void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    var dirInfo = new DirectoryInfo(path);
                    foreach (FileInfo finfo in dirInfo.GetFiles())
                    {
                        finfo.Delete();
                    }

                    foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                    {
                       RecursiveDelete(dir);
                    }

                    //delete in reverse, or directory throws an exception for having contents (this is my hack)
                    for (int i = (_redundantfolders.Count - 1); i >= 0; i--)
                    {
                        _redundantfolders[i].Delete();
                    }

                    if (!dirInfo.GetFiles().Any())
                    {
                        Directory.Delete(path);
                    }
                    _redundantfolders.Clear();
                }
                else
                {
                    MessageBox.Show("Unexpected event happened. Either no access to folder, or its deleted/moved!", "Fault!");
                }
                
            }
            catch (Exception ex)
            {
                //new ElegantMessageBox(this, ex.StackTrace, MessageType.Critical);
            }
        }


        /// <summary>
        /// Recursive function to delete subfolders and files
        /// </summary>
        private readonly List<DirectoryInfo> _redundantfolders = new List<DirectoryInfo>();
        private void RecursiveDelete(DirectoryInfo dirInfo)
        {
           
            foreach (FileInfo finfo in dirInfo.GetFiles())
            {
                finfo.Delete();
            }

            if (!dirInfo.GetDirectories().Any())
            {
                dirInfo.Delete();

            }
            else
            {
                _redundantfolders.Add(dirInfo);
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    RecursiveDelete(dir);
                }
            }
        }
    }
}
