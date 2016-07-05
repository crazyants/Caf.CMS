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
    using System.Net.Mail;
    using System.Text;

    internal class MailLogProvider : ILogProvider {
        private readonly string _from;
        private readonly string _to;
        private readonly string _subject;

        public Logger.Severity Treshold { get; set; }

        private MailLogProvider(string from, string to, string subject, Logger.Severity treshold) {
            _from = from;
            _to = to;
            _subject = subject;
            Treshold = treshold;
        }

        public void Write(LogItem item) {
            string formattedMessage = MessageFormatter.Format(item);
            SendMail(formattedMessage);
        }

        private void SendMail(string formattedMessage) {
            SendMail(_from, _to, _subject, formattedMessage, true);
        }

        private static void SendMail(string from, string to, string subject, string body, bool isHtml) {
            try {
                MailMessage message = new MailMessage(from, to, subject, body) {
                                                                                   BodyEncoding = Encoding.UTF8,
                                                                                   SubjectEncoding = Encoding.UTF8,
                                                                                   IsBodyHtml = isHtml
                                                                               };
                new SmtpClient().Send(message);
            }
            catch {
                // TODO: What to do? Log it? :)
            }
        }

        internal static bool IsConfigured {
            get {
                LoggersSection configurationManager = LoggersSection.GetFromConfiguration();
                if (configurationManager.MailLogger == null) {
                    return false;
                }

                return true;
            }
        }

        internal static ILogProvider CreateFromConfiguration() {
            LoggersSection configurationManager = LoggersSection.GetFromConfiguration();
            MailLoggerElement fileLoggerElement = configurationManager.MailLogger;

            string from = fileLoggerElement.From;
            string to = fileLoggerElement.To;
            string subject = fileLoggerElement.Subject;
            Logger.Severity treshold = fileLoggerElement.Treshold;

            return new MailLogProvider(from, to, subject, treshold);
        }
    }
}