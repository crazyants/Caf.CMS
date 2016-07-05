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

namespace CAF.Infrastructure.Core.Logger
{
    using System;
    using System.Collections.Generic;
    using LogProviders;

    public class Logger {
        public enum Severity {
            NotSet = 0,
            Info = 1,
            Warning = 2,
            Minor = 3,
            Major = 4,
            Critical = 5
        }

        private static readonly List<ILogProvider> LogProviders;

        static Logger() {
            LogProviders = new List<ILogProvider>();

            AddLogProvidersFromConfiguration();
        }

        private static void AddLogProvidersFromConfiguration() {
            if (FileLogProvider.IsConfigured) {
                LogProviders.Add(FileLogProvider.CreateFromConfiguration());
            }
            if (MailLogProvider.IsConfigured) {
                LogProviders.Add(MailLogProvider.CreateFromConfiguration());
            }
            if (DebugLogProvider.IsConfigured) {
                LogProviders.Add(DebugLogProvider.CreateFromConfiguration());
            }
        }

        public static void Write(Exception exception) {
            LogItem logItem = new LogItem(exception, Severity.NotSet);

            WriteToProviders(logItem);
        }

        public static void Write(Exception exception, Severity serverity) {
            LogItem logItem = new LogItem(exception, serverity);

            WriteToProviders(serverity, logItem);
        }

        public static void Write(string message) {
            LogItem logItem = new LogItem(message, Severity.NotSet);

            WriteToProviders(logItem);
        }

        public static void Write(string message, Severity serverity) {
            LogItem logItem = new LogItem(message, serverity);

            WriteToProviders(serverity, logItem);
        }

        private static void WriteToProviders(LogItem logItem) {
            foreach (ILogProvider logProvider in LogProviders) {
                logProvider.Write(logItem);
            }
        }

        private static void WriteToProviders(Severity serverity, LogItem logItem) {
            foreach (ILogProvider logProvider in LogProviders) {
                if (serverity >= logProvider.Treshold) {
                    logProvider.Write(logItem);
                }
            }
        }
    }
}

