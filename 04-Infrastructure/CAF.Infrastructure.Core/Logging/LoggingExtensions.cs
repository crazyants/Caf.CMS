
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using System;

namespace CAF.Infrastructure.Core.Logging
{
    public static class LoggingExtensions
    {

        public static void Debug(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, user);
        }

        public static void Information(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, user);
        }

        public static void Warning(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, user);
        }

        public static void Error(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, user);
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, user);
        }

        public static void Error(this ILogger logger, Exception exception, User user = null)
        {
            FilteredLog(logger, LogLevel.Error, exception.Message, exception, user);
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null, User user = null)
        {
            // don't log thread abort exception
            if ((exception != null) && (exception is System.Threading.ThreadAbortException))
                return;

            if (logger.IsEnabled(level))
            {
                string fullMessage = exception == null ? string.Empty : exception.ToString();
                logger.InsertLog(level, message, fullMessage, user);
            }
        }
    }
}
