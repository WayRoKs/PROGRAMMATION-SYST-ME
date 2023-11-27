using System.Collections.Generic;
using PROGRAMMATION_SYST_ME.Model;
using System.IO;
using System.Text.RegularExpressions;
using PROGRAMMATION_SYST_ME.View;

namespace PROGRAMMATION_SYST_ME.ViewModel
{
    class UserInteractionViewModel
    {
        public List<BackupJobDataModel> BackupJobsData { set; get; } = new List<BackupJobDataModel>();
        public BackupJobModel BackupJobs { set; get; }
        public LogModel LogFile { set; get; } = new LogModel();
        private readonly StatusView statusView = new StatusView();
        private long totalSaveSize = 0;
        public UserInteractionViewModel() 
        {
            BackupJobs = new BackupJobModel(BackupJobsData);
        }
        /// <summary>
        /// Method to update backup jobs
        /// </summary>
        /// <param name="jobChoice"></param>
        /// <param name="change"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public int UpdateJob(int jobChoice, string change, string newValue) 
        {   // Utilisation d'un switch case
            if (change == "N")
            {
                BackupJobsData[jobChoice].Name = newValue;
            }
            else if (change == "S")
            {
                BackupJobsData[jobChoice].Source = newValue;
            }
            else if (change == "D")
            {
                BackupJobsData[jobChoice].Destination = newValue;
            }
            else if (change == "T")
            {
                BackupJobsData[jobChoice].Type = int.Parse(newValue);
            }
            BackupJobs.SaveParam(BackupJobsData);
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
            for (var i = 0; i < jobsToExec.Count; i++) // Utilisation de foreach
            { 
                if (errorCode == 0)
                {
                    statusView.JobStart(BackupJobsData[i].Name);
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    totalSaveSize = 0;
                    if (BackupJobsData[jobsToExec[i]].Type == 0) // Full backup
                    {
                        errorCode = FullCopy(BackupJobsData[jobsToExec[i]].Source, BackupJobsData[jobsToExec[i]].Destination);
                    }
                    else // Differencial backup
                    {
                        errorCode = DiferencialCopy(BackupJobsData[jobsToExec[i]].Source, BackupJobsData[jobsToExec[i]].Destination);
                    }
                    watch.Stop();
                    LogFile.WriteLogSave(
                        BackupJobsData[jobsToExec[i]],
                        watch.ElapsedMilliseconds, 
                        totalSaveSize
                    );
                    if (errorCode == 0)
                        statusView.JobStop(BackupJobsData[i].Name, watch.ElapsedMilliseconds);
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
                totalSaveSize += file.Length;
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
                    totalSaveSize += file.Length;
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
