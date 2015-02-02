using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Untangled
{
    /// <summary>
    /// XML generator for configurations
    /// </summary>
    [XmlRoot("Configuration")]
    public class ConfigurationLoader
    {
        [XmlElement ("Folderpath")]
        public string FolderPath { get; set; }
        [XmlElement("ClockTime")]
        public double ClockTime { get; set; }
    }

}
