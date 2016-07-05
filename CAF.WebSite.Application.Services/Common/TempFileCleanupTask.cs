

using CAF.Infrastructure.Core.Utilities;
using CAF.WebSite.Application.Services.Tasks;
namespace CAF.WebSite.Application.Services.Common
{
	/// <summary>
	/// Task to cleanup temporary files
	/// </summary>
	public partial class TempFileCleanupTask : ITask
	{
		public void Execute(TaskExecutionContext ctx)
		{
			FileSystemHelper.TempCleanup();
		}
	}
}
