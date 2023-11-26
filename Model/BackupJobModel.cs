using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PROGRAMMATION_SYST_ME.Model
{
    /// <summary>
    /// Model class for a list of 5 backup jobs
    /// </summary>
    class BackupJobModel
    {
        public XmlDocument xml = new XmlDocument();
        private readonly string xmlPath;
        public BackupJobModel(List<BackupJobDataModel> jobList)
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
                BackupJobDataModel data = new BackupJobDataModel();
                data.Name = node.ChildNodes[0].InnerText;
                data.Source = node.ChildNodes[1].InnerText;
                data.Destination = node.ChildNodes[2].InnerText;
                data.Type = int.Parse(node.ChildNodes[3].InnerText);
                jobList.Add(data);
                i++;
            }
        }
        
        /// <summary>
        /// Method that allows for edits in the backup jobs
        /// </summary>
        public void SaveParam(List<BackupJobDataModel> jobList)
        {
            var i = 0;
            foreach (XmlNode node in xml.DocumentElement)
            {
                node.ChildNodes[0].InnerText = jobList[i].Name;
                node.ChildNodes[1].InnerText = jobList[i].Source;
                node.ChildNodes[2].InnerText = jobList[i].Destination;
                node.ChildNodes[3].InnerText = jobList[i].Type.ToString();
                i++;
            }
            xml.Save(xmlPath);
        }
    }
    class BackupJobDataModel
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int Type { get; set; }// O is full backup and 1 is differential backup
    }
}
