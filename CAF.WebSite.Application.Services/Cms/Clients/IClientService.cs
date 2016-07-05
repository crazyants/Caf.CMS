using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Clients;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Clients
{
    /// <summary>
    /// Client service
    /// </summary>
    public partial interface IClientService
    {
        /// <summary>
        /// Deletes a client
        /// </summary>
        /// <param name="client">Client</param>
        void DeleteClient(Client client);

        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Client collection</returns>
        IList<Client> GetAllClients(bool showHidden = false);

        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="clientName">Client name</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Client collection</returns>
        IList<Client> GetAllClients(string clientName, bool showHidden = false);
        
        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="clientName">Client name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Clients</returns>
        IPagedList<Client> GetAllClients(string clientName,
            int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets a client
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <returns>Client</returns>
        Client GetClientById(int clientId);

        /// <summary>
        /// Inserts a client
        /// </summary>
        /// <param name="client">Client</param>
        void InsertClient(Client client);

        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="client">Client</param>
        void UpdateClient(Client client);

        
    }
}
