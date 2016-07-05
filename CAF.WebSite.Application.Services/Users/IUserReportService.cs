
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
 

namespace CAF.WebSite.Application.Services.Users
{
    /// <summary>
    /// User report service interface
    /// </summary>
    public partial interface IUserReportService
    {
        /// <summary>
        /// Gets a report of users registered in the last days
        /// </summary>
        /// <param name="days">Users registered in the last days</param>
        /// <returns>Number of registered users</returns>
        int GetRegisteredUsersReport(int days);
    }
}