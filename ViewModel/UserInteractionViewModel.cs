using System.Collections.Generic;
using PROGRAMMATION_SYST_ME.Model;
using System.IO;
using System;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace PROGRAMMATION_SYST_ME.ViewModel
{
    class UserInteractionViewModel
    {
        public BackupJobsModel backupJobs = new BackupJobsModel();
        public LogModel log = new LogModel();
        private long saveSize = 0;
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
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    saveSize = 0;
                    if (backupJobs.Type[jobsToExec[i]] == 0) // Full backup
                    {
                        errorCode = FullCopy(backupJobs.Source[jobsToExec[i]], backupJobs.Destination[jobsToExec[i]]);
                    }
                    else // Differencial backup
                    {
                        errorCode = DiferencialCopy(backupJobs.Source[jobsToExec[i]], backupJobs.Destination[jobsToExec[i]]);
                    }
                    watch.Stop();
                    log.WriteLogSave(backupJobs.Name[jobsToExec[i]],
                        backupJobs.Source[jobsToExec[i]],
                        backupJobs.Destination[jobsToExec[i]],
                        backupJobs.Type[jobsToExec[i]],
                        watch.ElapsedMilliseconds, 
                        saveSize);
                }
                else 
                    break;
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
                saveSize += file.Length;
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
                {
                    file.CopyTo(destPath, true);
                    saveSize += file.Length;
                }
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
