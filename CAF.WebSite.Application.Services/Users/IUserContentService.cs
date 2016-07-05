using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Users
{
    /// <summary>
    /// User content service interface
    /// </summary>
    public partial interface IUserContentService
    {
        /// <summary>
        /// Deletes a user content
        /// </summary>
        /// <param name="content">User content</param>
        void DeleteUserContent(UserContent content);

        /// <summary>
        /// Gets all user content
        /// </summary>
        /// <param name="userId">User identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <returns>User content</returns>
        IList<UserContent> GetAllUserContent(int userId, bool? approved);
        
        /// <summary>
        /// Gets all user content
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="userId">User identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <returns>User content</returns>
        IList<T> GetAllUserContent<T>(int userId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null) where T : UserContent;

        /// <summary>
        /// Gets a user content
        /// </summary>
        /// <param name="contentId">User content identifier</param>
        /// <returns>User content</returns>
        UserContent GetUserContentById(int contentId);

        /// <summary>
        /// Inserts a user content
        /// </summary>
        /// <param name="content">User content</param>
        void InsertUserContent(UserContent content);

        /// <summary>
        /// Updates a user content
        /// </summary>
        /// <param name="content">User content</param>
        void UpdateUserContent(UserContent content);
    }
}
