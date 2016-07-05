
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.Infrastructure.Core.Logging;
using System;
 

namespace CAF.WebSite.Application.Services.Tasks
{
    /// <summary>
    /// Represents a task for deleting log entries
    /// </summary>
    public partial class ClearLogsTask : ITask
    {
        private readonly ILogger _logger;

        public ClearLogsTask(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute(TaskExecutionContext ctx)
        {
            var olderThanDays = 7; // TODO: move to settings
            var toUtc = DateTime.UtcNow.AddDays(-olderThanDays);

			_logger.ClearLog(toUtc, LogLevel.Error);
        }
    }
}
