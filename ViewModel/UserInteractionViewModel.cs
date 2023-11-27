using System.Collections.Generic;
using PROGRAMMATION_SYST_ME.Model;
using System.IO;
using System.Text.RegularExpressions;
using PROGRAMMATION_SYST_ME.View;

namespace PROGRAMMATION_SYST_ME.ViewModel
{
    class UserInteractionViewModel
    {
        public List<BackupJobDataModel> backupJobsData = new List<BackupJobDataModel>();
        public BackupJobModel backupJobs;
        public LogModel logFile = new LogModel();
        private StatusView statusView = new StatusView();
        private long saveSize = 0;
        public UserInteractionViewModel() 
        {
            backupJobs = new BackupJobModel(backupJobsData);
        }
        /// <summary>
        /// Method to update backup jobs
        /// </summary>
        /// <param name="jobChoice"></param>
        /// <param name="change"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public int UpdateJob(int jobChoice, string change, string newValue) 
        {
            if (change == "N")
            {
                backupJobsData[jobChoice].Name = newValue;
            }
            else if (change == "S")
            {
                backupJobsData[jobChoice].Source = newValue;
            }
            else if (change == "D")
            {
                backupJobsData[jobChoice].Destination = newValue;
            }
            else if (change == "T")
            {
                backupJobsData[jobChoice].Type = int.Parse(newValue);
            }
            backupJobs.SaveParam(backupJobsData);
            return 0;
        }
        /// <summary>
        /// Method to execute backup jobs
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public int ExecuteJob(string selection) // execute save job
        {
            var errorCode = 0;
            List<int> jobsToExec = new List<int>();
            if (Regex.IsMatch(selection, @"^[1-4]-[2-5]\z"))
            {
                var start = selection[0];
                var end = selection[2];
                for (var i = start; i < end + 1; i++)
                {
                    jobsToExec.Add(i - '0' - 1);
                }
            }
            else if (Regex.IsMatch(selection, @"^[1-5](;[1-5]){0,3};[1-5]\z"))
            {
                foreach (char c in selection)
                {
                    if (c != ';')
                        jobsToExec.Add(c - '0' - 1);
                }
            }
            else if (Regex.IsMatch(selection, @"^[1-5]\z"))
            {
                jobsToExec.Add(int.Parse(selection) - 1);
            }
            else if (selection == "Q")
                return 0;
            else
            {
                return 2;
            }
            for (var i = 0; i < jobsToExec.Count; i++)
            { 
                if (errorCode == 0)
                {
                    statusView.JobStart(backupJobsData[i].Name);
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    saveSize = 0;
                    if (backupJobsData[jobsToExec[i]].Type == 0) // Full backup
                    {
                        errorCode = FullCopy(backupJobsData[jobsToExec[i]].Source, backupJobsData[jobsToExec[i]].Destination);
                    }
                    else // Differencial backup
                    {
                        errorCode = DiferencialCopy(backupJobsData[jobsToExec[i]].Source, backupJobsData[jobsToExec[i]].Destination);
                    }
                    watch.Stop();
                    logFile.WriteLogSave(
                        backupJobsData[jobsToExec[i]],
                        watch.ElapsedMilliseconds, 
                        saveSize
                    );
                    if (errorCode == 0)
                        statusView.JobStop(backupJobsData[i].Name, watch.ElapsedMilliseconds);
                }
                else 
                    break;
            }
            if (errorCode == 0)
            statusView.JobsComplete();
            return errorCode;
        }
        /// <summary>
        /// Method to execute a full copy
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
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
                saveSize += file.Length;
            }
            foreach (DirectoryInfo subDir in dirs)
            {
                FullCopy(subDir.FullName, Path.Combine(destination, subDir.Name));
            }
            return 0;
        }
        /// <summary>
        /// Method to execute a differencial copy
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
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
                {
                    file.CopyTo(destPath, true);
                    saveSize += file.Length;
                }
            }
            foreach (DirectoryInfo subDir in dirs)
            {
                DiferencialCopy(subDir.FullName, Path.Combine(destination, subDir.Name));
            }
            return 0;

        }
    }
}
