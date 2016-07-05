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

    internal class LogItem {

        public LogItem(Exception exception, Logger.Severity severity) {
            TimeStamp = DateTime.Now;
            Message = exception.Message;
            CallStack = exception.StackTrace;
            Severity = severity;

            RethrowIfErrorOccursWhileLogging = true;
        }

        public LogItem(string message, Logger.Severity severity) {
            TimeStamp = DateTime.Now;
            Message = message;
            Severity = severity;

            RethrowIfErrorOccursWhileLogging = true;
        }

        internal DateTime TimeStamp { get; private set; }
        internal string Message { get; private set; }
        internal string CallStack { get; private set; }
        internal Logger.Severity Severity { get; private set; }
        internal bool RethrowIfErrorOccursWhileLogging { get; set; }
    }
}
