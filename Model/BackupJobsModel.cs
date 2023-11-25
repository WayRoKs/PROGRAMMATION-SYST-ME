using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace PROGRAMMATION_SYST_ME.Model
{
    /// <summary>
    /// Model class for a list of 5 backup jobs
    /// </summary>
    class BackupJobsModel
    {
        public XmlDocument xml = new XmlDocument();
        private readonly string xmlPath;
        public BackupJobsModel()
        {
            xmlPath = Path.Combine(Environment.CurrentDirectory, @"SaveJobsConfig.xml");
            // if the selected path is found, proceed. otherwise raise an error.
            try
            {  
                xml.Load(xmlPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e}");
                Environment.Exit(3);
            }
            var i = 0;
            foreach (XmlNode node in xml.DocumentElement)
            {
                Name[i] = node.ChildNodes[0].InnerText;
                Source[i] = node.ChildNodes[1].InnerText;
                Destination[i] = node.ChildNodes[2].InnerText;
                Type[i] = int.Parse(node.ChildNodes[3].InnerText);
                i++;
            }
        }
        public string[] Name {  get; set; } = new string[5];
        public string[] Source { get; set; } = new string[5];
        public string[] Destination { get; set; } = new string[5];
        public int[] Type { get; set; } = new int[5];// O is full backup and 1 is differential backup
        /// <summary>
        /// Method that allows for edits in the backup jobs
        /// </summary>
        public void SaveParam()
        {
            var i = 0;
            foreach (XmlNode node in xml.DocumentElement)
            {
                node.ChildNodes[0].InnerText = Name[i];
                node.ChildNodes[1].InnerText = Source[i];
                node.ChildNodes[2].InnerText = Destination[i];
                node.ChildNodes[3].InnerText = Type[i].ToString();
                i++;
            }
            xml.Save(xmlPath);
        }
    }
}
