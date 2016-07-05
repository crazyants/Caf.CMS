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
    using System.Diagnostics;
    using System.Text;

    internal static class MessageFormatter {

        private static string GetOrCreateStackTrace(Exception exception) {
            if (string.IsNullOrEmpty(exception.StackTrace)) {
                var stackTrace = new StackTrace();
                return stackTrace.ToString();
            }

            return exception.StackTrace;
        }

        private static string GetTimeStamp(DateTime currentDate) {
            return string.Format("{0} {1}", currentDate.ToShortDateString(), currentDate.ToShortTimeString());
        }

        public static string Format(LogItem item) {
            StringBuilder stringBuilder = new StringBuilder();

            string timeStamp = GetTimeStamp(item.TimeStamp);
            
            stringBuilder.Append(string.Format("{0} [{1}] {2}\r\n", timeStamp, item.Severity, item.Message));

            if(!string.IsNullOrEmpty(item.CallStack)) {
                stringBuilder.Append(item.CallStack);
            }

            return stringBuilder.ToString();
        }
    }
}