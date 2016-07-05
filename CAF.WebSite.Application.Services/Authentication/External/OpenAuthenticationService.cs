//Contributor:  Nicholas Mayne

using System;
using System.Collections.Generic;
using System.Linq;

using CAF.WebSite.Application.Services.Configuration;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Configuration;
 

namespace CAF.WebSite.Application.Services.Authentication.External
{
    public partial class OpenAuthenticationService : IOpenAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
		private readonly ISettingService _settingService;
		private readonly IProviderManager _providerManager;

        public OpenAuthenticationService(
			IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IPluginFinder pluginFinder,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IUserService userService,
			ISettingService settingService,
			IProviderManager providerManager)
        {
            this._externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            this._pluginFinder = pluginFinder;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._userService = userService;
			this._settingService = settingService;
			this._providerManager = providerManager;
        }

		/// <summary>
		/// Load all external authentication methods
		/// </summary>
		/// <param name="storeId">Load records allows only in specified store; pass 0 to load all records</param>
		/// <returns>External authentication methods</returns>
		public virtual IEnumerable<Provider<IExternalAuthenticationMethod>> LoadAllExternalAuthenticationMethods(int storeId = 0)
		{
			return _providerManager.GetAllProviders<IExternalAuthenticationMethod>(storeId);
		}

        /// <summary>
        /// Load active external authentication methods
        /// </summary>
		/// <param name="storeId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Payment methods</returns>
		public virtual IEnumerable<Provider<IExternalAuthenticationMethod>> LoadActiveExternalAuthenticationMethods(int storeId = 0)
        {
			var allMethods = LoadAllExternalAuthenticationMethods(storeId);
			var activeMethods = allMethods
				   .Where(p => _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Contains(p.Metadata.SystemName, StringComparer.InvariantCultureIgnoreCase));

			return activeMethods;
        }

        /// <summary>
        /// Load external authentication method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found external authentication method</returns>
		public virtual Provider<IExternalAuthenticationMethod> LoadExternalAuthenticationMethodBySystemName(string systemName, int storeId = 0)
        {
			return _providerManager.GetProvider<IExternalAuthenticationMethod>(systemName, storeId);
        }




        public virtual void AssociateExternalAccountWithUser(User user, OpenAuthenticationParameters parameters)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            //find email
            string email = null;
            if (parameters.UserClaims != null)
                foreach (var userClaim in parameters.UserClaims
                    .Where(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Email)))
                    {
                        //found
                        email = userClaim.Contact.Email;
                        break;
                    }

            var externalAuthenticationRecord = new ExternalAuthenticationRecord()
            {
                UserId = user.Id,
                Email = email,
                ExternalIdentifier = parameters.ExternalIdentifier,
                ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
                OAuthToken = parameters.OAuthToken,
                OAuthAccessToken = parameters.OAuthAccessToken,
                ProviderSystemName = parameters.ProviderSystemName,
            };

            _externalAuthenticationRecordRepository.Insert(externalAuthenticationRecord);
        }

        public virtual bool AccountExists(OpenAuthenticationParameters parameters)
        {
            return GetUser(parameters) != null;
        }

        public virtual User GetUser(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .Where(o => o.ExternalIdentifier == parameters.ExternalIdentifier && o.ProviderSystemName == parameters.ProviderSystemName)
                .FirstOrDefault();

            if (record != null)
                return _userService.GetUserById(record.UserId);

            return null;
        }

        public virtual IList<ExternalAuthenticationRecord> GetExternalIdentifiersFor(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return user.ExternalAuthenticationRecords.ToList();
        }

        public virtual void RemoveAssociation(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .Where(o => o.ExternalIdentifier == parameters.ExternalIdentifier && o.ProviderSystemName == parameters.ProviderSystemName)
                .FirstOrDefault();

            if (record != null)
                _externalAuthenticationRecordRepository.Delete(record);
        }
    }
}