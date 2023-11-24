using System.Collections.Generic;
using PROGRAMMATION_SYST_ME.Model;
using System.IO;

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
            var errorCode = 0;
            for (var i = 0; i < jobsToExec.Count; i++)
            {
                if (errorCode == 0)
                {
                    if (backupJobs.Type[jobsToExec[i]] == 0) // Full backup
                    {
                        errorCode = FullCopy(backupJobs.Source[jobsToExec[i]], backupJobs.Destination[jobsToExec[i]]);
                    }
                    else // Differencial backup
                    {
                        errorCode = DiferencialCopy(backupJobs.Source[jobsToExec[i]], backupJobs.Destination[jobsToExec[i]]);
                    }
                }
                else break;
            }
            return errorCode;
        }
        public int FullCopy(string source, string destination)
        {
            var dir = new DirectoryInfo(source);
            if (!dir.Exists)
                return 3;
            DirectoryInfo[] dirs = dir.GetDirectories();
            var dirDest = new DirectoryInfo(destination);
            if (dirDest.Exists)
                Directory.Delete(destination, true);
            Directory.CreateDirectory(destination);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.CopyTo(Path.Combine(destination, file.Name), true);
            }
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destination, subDir.Name);
                FullCopy(subDir.FullName, newDestinationDir);
            }
            return 0;
        }
        public int DiferencialCopy(string source, string destination)
        {
            var dir = new DirectoryInfo(source);
            if (!dir.Exists)
                return 3;
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destination);
            foreach (FileInfo file in dir.GetFiles())
            {
                var destPath = Path.Combine(destination, file.Name);
                var destFile = new FileInfo(destPath);
                if (file.LastWriteTime != destFile.LastWriteTime) // Condition to see if file changed
                    file.CopyTo(destPath, true);
            }
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destination, subDir.Name);
                DiferencialCopy(subDir.FullName, newDestinationDir);
            }
            return 0;

        }
    }
}
