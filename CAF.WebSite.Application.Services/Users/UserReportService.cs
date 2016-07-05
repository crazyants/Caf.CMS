using CAF.Infrastructure.Core.Data;
using CAF.WebSite.Application.Services.Helpers;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
 

namespace CAF.WebSite.Application.Services.Users
{
    /// <summary>
    /// User report service
    /// </summary>
    public partial class UserReportService : IUserReportService
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        
        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userRepository">User repository</param>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="userService">User service</param>
        /// <param name="dateTimeHelper">Date time helper</param>
        public UserReportService(IRepository<User> userRepository,
            IUserService userService,
            IDateTimeHelper dateTimeHelper)
        {
            this._userRepository = userRepository;
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        
        /// <summary>
        /// Gets a report of users registered in the last days
        /// </summary>
        /// <param name="days">Users registered in the last days</param>
        /// <returns>Number of registered users</returns>
        public virtual int GetRegisteredUsersReport(int days)
        {
            DateTime date = _dateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            var registeredUserRole = _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered);
            if (registeredUserRole == null)
                return 0;

            var query = from c in _userRepository.Table
                        from cr in c.UserRoles
                        where !c.Deleted &&
                        cr.Id == registeredUserRole.Id &&
                        c.CreatedOnUtc >= date 
                        //&& c.CreatedOnUtc <= DateTime.UtcNow
                        select c;
            int count = query.Count();
            return count;
        }

        #endregion
    }
}