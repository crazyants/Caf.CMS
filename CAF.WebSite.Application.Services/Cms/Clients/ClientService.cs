using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Clients;
namespace CAF.WebSite.Application.Services.Clients
{
    /// <summary>
    /// Client service
    /// </summary>
    public partial class ClientService : IClientService
    {
        #region Constants
        private const string PRODUCTMANUFACTURERS_ALLBYMANUFACTURERID_KEY = "WebSite.productclient.allbyclientid-{0}-{1}-{2}-{3}-{4}";
        private const string PRODUCTMANUFACTURERS_ALLBYPRODUCTID_KEY = "WebSite.productclient.allbyproductid-{0}-{1}-{2}";
        private const string MANUFACTURERS_PATTERN_KEY = "WebSite.client.";
        private const string MANUFACTURERS_BY_ID_KEY = "WebSite.client.id-{0}";
        private const string PRODUCTMANUFACTURERS_PATTERN_KEY = "WebSite.productclient.";

        #endregion

        #region Fields

        private readonly IRepository<Client> _clientRepository;
		private readonly IRepository<SiteMapping> _siteMappingRepository;
		private readonly IWorkContext _workContext;
		private readonly ISiteContext _siteContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="clientRepository">Category repository</param>
        /// <param name="productClientRepository">ProductCategory repository</param>
        /// <param name="productRepository">Product repository</param>
		/// <param name="siteMappingRepository">Site mapping repository</param>
		/// <param name="workContext">Work context</param>
		/// <param name="siteContext">Site context</param>
        /// <param name="eventPublisher">Event published</param>
        public ClientService(ICacheManager cacheManager,
            IRepository<Client> clientRepository,
			IRepository<SiteMapping> siteMappingRepository,
			IWorkContext workContext,
			ISiteContext siteContext,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _clientRepository = clientRepository;
			_siteMappingRepository = siteMappingRepository;
			_workContext = workContext;
			_siteContext = siteContext;
            _eventPublisher = eventPublisher;

			this.QuerySettings = DbQuerySettings.Default;
		}

		public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a client
        /// </summary>
        /// <param name="client">Client</param>
        public virtual void DeleteClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            
            client.Deleted = true;
            UpdateClient(client);
        }

        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Client collection</returns>
        public virtual IList<Client> GetAllClients(bool showHidden = false)
        {
            return GetAllClients(null, showHidden);
        }

        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="clientName">Client name</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Client collection</returns>
        public virtual IList<Client> GetAllClients(string clientName, bool showHidden = false)
        {
            var query = _clientRepository.Table;
            if (!showHidden)
                query = query.Where(m => m.Published);
            if (!String.IsNullOrWhiteSpace(clientName))
                query = query.Where(m => m.Name.Contains(clientName));
            query = query.Where(m => !m.Deleted);
            query = query.OrderBy(m => m.DisplayOrder);

			//Site mapping
			if (!showHidden)
			{
				//Site mapping
				if (!QuerySettings.IgnoreMultiSite)
				{
					var currentSiteId = _siteContext.CurrentSite.Id;
					query = from m in query
							join sm in _siteMappingRepository.Table
							on new { c1 = m.Id, c2 = "Client" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
							from sm in m_sm.DefaultIfEmpty()
							where !m.LimitedToSites || currentSiteId == sm.SiteId
							select m;
				}

				//only distinct clients (group by ID)
				query = from m in query
						group m by m.Id	into mGroup
						orderby mGroup.Key
						select mGroup.FirstOrDefault();

				query = query.OrderBy(m => m.DisplayOrder);
			}

            var clients = query.ToList();
            return clients;
        }
        
        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="clientName">Client name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Clients</returns>
        public virtual IPagedList<Client> GetAllClients(string clientName,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var clients = GetAllClients(clientName, showHidden);
            return new PagedList<Client>(clients, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a client
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <returns>Client</returns>
        public virtual Client GetClientById(int clientId)
        {
            if (clientId == 0)
                return null;

            string key = string.Format(MANUFACTURERS_BY_ID_KEY, clientId);
            return _cacheManager.Get(key, () => { 
                return _clientRepository.GetById(clientId); 
            });
        }

        /// <summary>
        /// Inserts a client
        /// </summary>
        /// <param name="client">Client</param>
        public virtual void InsertClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _clientRepository.Insert(client);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(client);
        }

        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="client">Client</param>
        public virtual void UpdateClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _clientRepository.Update(client);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(client);
        }

      
        #endregion
    }
}
