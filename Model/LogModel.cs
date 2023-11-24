using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace PROGRAMMATION_SYST_ME.Model
{
    /// <summary>
    /// Log model class
    /// </summary>
    class LogModel
    {
        private readonly string logFolder = "C:\\SaveLog"; // Destination folder for the logs
        private readonly string logFile;
        public string Name { set; get; }
        public string Source { set; get; }
        public string Destination { set; get; }
        public int FileSize { set; get; }
        public double TransferTime { set; get; }
        public DateTime Time { set; get; }
        /// <summary>
        /// When creating a .json log file, the name is automatimacally generated to match the current date
        /// </summary>
        public LogModel()
        {
            logFile = Path.Combine(logFolder, DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + ".json");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"> Info </param>
        /// <param name="elapsedTime"> Time </param>
        /// <param name="fileSize"> Size of the log file </param>
        public void WriteLogSave(string name, string source, string destination, int type, long elapsedTime, long saveSize)
        {
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);
            Data data = new Data()
            {
                Name = name,
                Source = source,
                Destination = destination,
                Type = type,
                ElapsedTime = elapsedTime,
                SaveSize = saveSize,
                Time = DateTime.Now.ToString()
            };
            var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.AppendAllText(logFile, jsonString);
        }
    }
    /// <summary
    ///
    ///
    class Data
    {
        public string Name { set; get; }
        public string Source { set; get; }
        public string Destination { set; get; }
        public int Type { set; get; } 
        public long ElapsedTime { set; get; }
        public long SaveSize { set; get; }
        public string Time { set; get; } 
    }
}
