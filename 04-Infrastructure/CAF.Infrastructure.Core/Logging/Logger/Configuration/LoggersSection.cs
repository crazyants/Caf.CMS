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

namespace CAF.Infrastructure.Core.Logger.Configuration
{
    using System.Configuration;

    public class LoggersSection : ConfigurationSection {
        private static LoggersSection _loggersSection = ConfigurationManager.GetSection("loggers") as LoggersSection;

        internal static LoggersSection GetFromConfiguration() {
            return _loggersSection;
        }

        [ConfigurationProperty("fileLogger", IsDefaultCollection = false)]
        public FileLoggerElement FileLogger {
            get { return (FileLoggerElement)base["fileLogger"]; }
        }

        [ConfigurationProperty("mailLogger", IsDefaultCollection = false)]
        public MailLoggerElement MailLogger {
            get { return (MailLoggerElement)base["mailLogger"]; }
        }

        [ConfigurationProperty("debugLogger", IsDefaultCollection = false)]
        public DebugLoggerElement DebugLogger {
            get { return (DebugLoggerElement)base["debugLogger"]; }
        }
    }
}