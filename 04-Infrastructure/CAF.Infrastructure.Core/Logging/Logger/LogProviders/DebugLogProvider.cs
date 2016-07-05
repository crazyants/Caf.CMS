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
    using Configuration;
    using System;
    using System.Diagnostics;

    //
    // NOTE: To enable this logger the project must be compiled in debug mode!
    //
    internal class DebugLogProvider : ILogProvider {
        internal DebugLogProvider(Logger.Severity treshold) {
            Treshold = treshold;
        }

        public Logger.Severity Treshold { get; set; }

        public void Write(LogItem item) {
            string formattedMessage = MessageFormatter.Format(item);
            Debug.Write(formattedMessage);
        }

        internal static bool IsConfigured {
            get {
                LoggersSection configurationManager = LoggersSection.GetFromConfiguration();
                if (configurationManager.DebugLogger == null) {
                    return false;
                }

                return true;
            }
        }

        internal static ILogProvider CreateFromConfiguration() {
            LoggersSection configurationManager = LoggersSection.GetFromConfiguration();
            DebugLoggerElement debugLoggerElement = configurationManager.DebugLogger;

            Logger.Severity treshold = debugLoggerElement.Treshold;

            return new DebugLogProvider(treshold);
        }
     }
}