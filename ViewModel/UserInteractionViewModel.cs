using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Reflection.Metadata;
using System.Text;
using PROGRAMMATION_SYST_ME.Model;
using System.Xml.Linq;

namespace PROGRAMMATION_SYST_ME.ViewModel
{
    class UserInteractionViewModel
    {
        public BackupJobsModel backupJobs = new BackupJobsModel();
        public int UpdateJob(int jobChoice, string change, string newValue) // update save job 
        {
            if (change == "N")
            {
                backupJobs.Name[jobChoice] = newValue;
            }
            else if (change == "S")
            {
                backupJobs.Source[jobChoice] = newValue;
            }
            else if (change == "D")
            {
                backupJobs.Destination[jobChoice] = newValue;
            }
            else if (change == "T")
            {
                backupJobs.Type[jobChoice] = int.Parse(newValue);
            }
            backupJobs.SaveParam();
            return 0;
        }
        public int ExecuteJob(List<int> jobsToExec) // execute save job
        {

            return 0;
        }
    }
}
