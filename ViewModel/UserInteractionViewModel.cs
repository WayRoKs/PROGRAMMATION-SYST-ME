using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection.Metadata;
using System.Text;
using PROGRAMMATION_SYST_ME.Model;

namespace PROGRAMMATION_SYST_ME.ViewModel
{
    class UserInteractionViewModel
    {
        public BackupJobsModel[] backupJobs;
        public UserInteractionViewModel()
        {
            XDocument xml = null;
            try
            {
                xml = XDocument.Load(@"C:\Users\nicor\source\repos\PROGRAMMATION-SYST-ME\SaveJobsConfig.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e}");
                Environment.Exit(3);
            }
            var names = (from val in xml.Root.Descendants("saveJobs1")
                        select val.Element("name").Value).ToList();
            for (int i = 0; i < 5; i++)
            {
                backupJobs[i] = new BackupJobsModel();
                backupJobs[i].Name = names.ElementAt(i);
                
                Console.WriteLine(backupJobs[i].Name);
            }
        }
        public int UpdateJob(int jobChoice, string change, string newValue) // update save job 
        {

            return 0;
        }
        public int ExecuteJob(string jobsToExec) // execute save job
        {

            return 0;
        }
    }
}
