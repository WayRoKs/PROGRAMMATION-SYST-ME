using System;
using System.IO;
using System.Text.Json;

namespace PROGRAMMATION_SYST_ME.Model
{
    /// <summary>
    /// Log model class
    /// </summary>
    class LogModel
    {
        private readonly string logFolder;
        private readonly string logFile;
        public int NbJobs;
        /// <summary>
        /// When creating a .json log file, the name is automatimacally generated to match the current date
        /// </summary>
        public LogModel()
        {
            logFolder = Path.Combine(Environment.CurrentDirectory, "logs");
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);
            logFile = Path.Combine(logFolder, DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + ".json");
        }
        /// <summary>
        /// Method to write our logs' content
        /// </summary>
        /// <param name="info"> Info </param>
        /// <param name="elapsedTime"> Time </param>
        /// <param name="fileSize"> Size of the log file </param>
        public void WriteLogSave(BackupJobDataModel logData, long elapsedTime, long saveSize)
        {
            LogDataModel data = new LogDataModel()
            {
                LogData = logData,
                ElapsedTime = elapsedTime,
                SaveSize = saveSize,
                Time = DateTime.Now.ToString()
            };
            var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.AppendAllText(logFile, jsonString);
        }
    }
    class LogDataModel
    {
        public BackupJobDataModel LogData { get; set;}
        public long ElapsedTime { get; set;}
        public long SaveSize { get; set;}
        public string Time { get; set;}
    }
}
