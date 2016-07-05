using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Data;
using CAF.WebSite.Application.Services.Tasks;
using CAF.Infrastructure.Core.Domain.Messages;
using System;
using System.Linq;
using System.Linq.Expressions;
 

namespace CAF.WebSite.Application.Services.Messages
{
    /// <summary>
    /// Represents a task for deleting sent emails from the message queue
    /// </summary>
    public partial class QueuedMessagesClearTask : ITask
    {
        private readonly IRepository<QueuedEmail> _qeRepository;

		public QueuedMessagesClearTask(IRepository<QueuedEmail> qeRepository)
        {
			this._qeRepository = qeRepository;
        }

		public void Execute(TaskExecutionContext ctx)
        {
			var olderThan = DateTime.UtcNow.AddDays(-14);
			_qeRepository.DeleteAll(x => x.SentOnUtc.HasValue && x.CreatedOnUtc < olderThan);

			_qeRepository.Context.ShrinkDatabase();
        }
    }
}
