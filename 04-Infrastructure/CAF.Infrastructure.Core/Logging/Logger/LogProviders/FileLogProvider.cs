#region License and copyright notice
/* 
 * Kaliko Logger
 * 
 * Copyright (c) 2011 Fredrik Schultz
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 */
#endregion

namespace CAF.Infrastructure.Core.Logger.LogProviders
{
    using System;
    using System.IO;
    using Configuration;

    internal class FileLogProvider : ILogProvider {
        private readonly string _logfile;
        
        public FileLogProvider(string filename, Logger.Severity treshold) {
            _logfile = ConvertToAbsolutePath(filename);
            Treshold = treshold;
        }

        private string ConvertToAbsolutePath(string fileName) {
            if (fileName.ToLower().StartsWith("|datadirectory|")) {
                string dataPath = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
                if (!string.IsNullOrEmpty(dataPath)) {
                    fileName = string.Format("{0}\\{1}", dataPath, fileName.Substring(15));
                }
            }

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseDirectory, fileName);
            return path;
        }

        public Logger.Severity Treshold { get; set; }

        public void Write(LogItem item) {
            string formattedMessage = MessageFormatter.Format(item);
            WriteToFile(formattedMessage);
        }

        private void WriteToFile(string formattedMessage) {
            string fileName = GetFileName();
            File.AppendAllText(fileName, formattedMessage);
        }

        private string GetFileName() {
            DateTime currentDate = DateTime.Now;
            string filename = _logfile;

            filename = filename.Replace("%yyyy", currentDate.Year.ToString());
            filename = filename.Replace("%mm", currentDate.Month.ToString());
            filename = filename.Replace("%dd", currentDate.Day.ToString());

            return filename;
        }

        public static bool IsConfigured {
            get {
                LoggersSection configurationManager = LoggersSection.GetFromConfiguration();
                if (configurationManager.FileLogger == null) {
                    return false;
                }

                return true;
            }
        }

        public static ILogProvider CreateFromConfiguration() {
            LoggersSection configurationManager = LoggersSection.GetFromConfiguration();
            FileLoggerElement fileLoggerElement = configurationManager.FileLogger;

            string filename = fileLoggerElement.Filename;
            Logger.Severity treshold = fileLoggerElement.Treshold;

            return new FileLogProvider(filename, treshold);
        }
    }
}