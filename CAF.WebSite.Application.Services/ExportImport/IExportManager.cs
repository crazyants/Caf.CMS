using System.Collections.Generic;
using System.IO;
using CAF.Infrastructure.Core.Domain.Users;

namespace CAF.WebSite.Application.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public interface IExportManager
    {
 


        /// <summary>
        /// Export user list to XLSX
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="users">Users</param>
        void ExportUsersToXlsx(Stream stream, IList<User> users);

        /// <summary>
        /// Export user list to xml
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>Result in XML format</returns>
        string ExportUsersToXml(IList<User> users);

       
    }
}
