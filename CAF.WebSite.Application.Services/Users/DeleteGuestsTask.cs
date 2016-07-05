
using CAF.WebSite.Application.Services.Tasks;
using System;
 

namespace CAF.WebSite.Application.Services.Users
{
    /// <summary>
    /// Represents a task for deleting guest users
    /// </summary>
    public partial class DeleteGuestsTask : ITask
    {
        private readonly IUserService _userService;

        public DeleteGuestsTask(IUserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute(TaskExecutionContext ctx)
        {
            //60*24 = 1 day
            var olderThanMinutes = 1440; // TODO: move to settings
            _userService.DeleteGuestUsers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
        }
    }
}
