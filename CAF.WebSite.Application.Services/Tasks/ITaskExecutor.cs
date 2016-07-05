using CAF.Infrastructure.Core.Domain.Tasks;
using System;


namespace CAF.WebSite.Application.Services.Tasks
{
    public interface ITaskExecutor
    {
        void Execute(ScheduleTask task, bool throwOnError = false);
    }
}
