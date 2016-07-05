using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Utilities;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Data;
using CAF.Infrastructure.Core.Email;
using CAF.WebSite.Application.Services.Common;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Cms.Payments;
using CAF.Infrastructure.Core.Domain.Cms.Topic;
using CAF.Infrastructure.Core.Domain;
using CAF.Infrastructure.Core.Domain.Cms.Polls;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Domain.Tasks;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Data.Setup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Domain.Configuration;
using CAF.WebSite.Application.Services.Configuration;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Common.Common;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Caching;

namespace CAF.WebSite.Mvc.Infrastructure.Installation
{
    public partial class InstallDataSeeder : IDataSeeder<DefaultObjectContext>
    {
        #region Fields & Constants

		private DefaultObjectContext _ctx;
        private SeedDataConfiguration _config;
        private InvariantSeedData _data;
		private ISettingService _settingService;
		private IGenericAttributeService _gaService;
		private IPictureService _pictureService;
		private ILocalizationService _locService;
		private IUrlRecordService _urlRecordService;
		private int _defaultSiteId;

        #endregion Fields & Constants

        #region Ctor

		public InstallDataSeeder(SeedDataConfiguration configuration)
        {
			Guard.ArgumentNotNull(() => configuration);

			Guard.ArgumentNotNull(configuration.Language, "Language");
			Guard.ArgumentNotNull(configuration.Data, "SeedData");

			_config = configuration;
			_data = configuration.Data;
        }

        #endregion Ctor

        #region Populate

		private void PopulateSites()
		{
			SaveRange(_data.Sites());
			_defaultSiteId = _data.Sites().First().Id;
		}

		private void PopulateTaxCategories()
        {
			SaveRange(_data.TaxCategories());
                
            // add tax rates to fixed rate provider
            var taxCategories = _ctx.Set<TaxCategory>().ToList();
            int i = 0;
            var taxIds = taxCategories.OrderBy(x => x.Id).Select(x => x.Id).ToList();
            foreach (var id in taxIds)
            {
                decimal rate = 0;
                if (_data.FixedTaxRates.Any() && _data.FixedTaxRates.Length > i)
                {
                    rate = _data.FixedTaxRates[i];
                }
                i++;
                this.SettingService.SetSetting(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", id), rate);
            }

			_ctx.SaveChanges();
        }

		private void PopulateLanguage(Language primaryLanguage)
        {
			primaryLanguage.Published = true;
			Save(primaryLanguage);
        }

        private void PopulateLocaleResources() 
        {
            // Default primary language
            var language = _ctx.Set<Language>().Single();

            var locPath = CommonHelper.MapPath("~/App_Data/Localization/App/" + language.LanguageCulture);
            if (!System.IO.Directory.Exists(locPath))
            {
                // Fallback to neutral language folder (de, en etc.)
				locPath = CommonHelper.MapPath("~/App_Data/Localization/App/" + language.UniqueSeoCode);
            }

			var localizationService = this.LocalizationService;

			// save resources
			foreach (var filePath in System.IO.Directory.EnumerateFiles(locPath, "*.smres.xml", SearchOption.TopDirectoryOnly))
			{
				var doc = new XmlDocument();
				doc.Load(filePath);

				doc = localizationService.FlattenResourceFile(doc);

				// now we have a parsed XML file (the same structure as exported language packs)
				// let's save resources
				localizationService.ImportResourcesFromXml(language, doc);

				// no need to call SaveChanges() here, as the above call makes it
				// already without AutoDetectChanges(), so it's fast.
			}

			MigratorUtils.ExecutePendingResourceMigrations(locPath, _ctx);
        }

	
		private void PopulateCountriesAndStates()
        {
			SaveRange(_data.Countries().Where(x => x != null));
        }

	

		private void PopulateUsersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var customerRoles = _data.UserRoles();
			SaveRange(customerRoles.Where(x => x != null));

            //admin user
            var adminUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                UserName = defaultUserEmail,
                Password = defaultUserPassword,
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            var adminAddress = _data.AdminAddress();

            adminUser.Addresses.Add(adminAddress);
            adminUser.BillingAddress = adminAddress;
            adminUser.ShippingAddress = adminAddress;
            adminUser.UserRoles.Add(customerRoles.SingleOrDefault(x => x.SystemName == SystemUserRoleNames.Administrators));
            adminUser.UserRoles.Add(customerRoles.SingleOrDefault(x => x.SystemName == SystemUserRoleNames.ForumModerators));
            adminUser.UserRoles.Add(customerRoles.SingleOrDefault(x => x.SystemName == SystemUserRoleNames.Registered));
            Save(adminUser);

			// Set default customer name
			this.GenericAttributeService.SaveAttribute(adminUser, SystemUserAttributeNames.FirstName, adminUser.Addresses.FirstOrDefault().FirstName);
			this.GenericAttributeService.SaveAttribute(adminUser, SystemUserAttributeNames.LastName, adminUser.Addresses.FirstOrDefault().LastName);
			_ctx.SaveChanges();

			// Built-in user for search engines (crawlers)
            var customer = _data.SearchEngineUser();
            customer.UserRoles.Add(customerRoles.SingleOrDefault(x => x.SystemName == SystemUserRoleNames.Guests));
            Save(customer);

            // Built-in user for background tasks
            customer = _data.BackgroundTaskUser();
            customer.UserRoles.Add(customerRoles.SingleOrDefault(x => x.SystemName == SystemUserRoleNames.Guests));
            Save(customer);

			// Built-in user for the PDF converter
			customer = _data.PdfConverterUser();
			customer.UserRoles.Add(customerRoles.SingleOrDefault(x => x.SystemName == SystemUserRoleNames.Guests));
			Save(customer);
        }

		private void HashDefaultUserPassword(string defaultUserEmail, string defaultUserPassword)
        {
			var adminUser = _ctx.Set<User>().Where(x => x.Email == _config.DefaultUserName).Single();

			var encryptionService = new EncryptionService(new SecuritySettings());

			string saltKey = encryptionService.CreateSaltKey(5);
			adminUser.PasswordSalt = saltKey;
			adminUser.PasswordFormat = PasswordFormat.Hashed;
			adminUser.Password = encryptionService.CreatePasswordHash(defaultUserPassword, saltKey, new UserSettings().HashedPasswordFormat);

			SetModified(adminUser);
			_ctx.SaveChanges();
        }

		private void PopulateSettings()
        {
			var method = typeof(ISettingService).GetMethods().FirstOrDefault(x =>
			{
				if (x.Name == "SaveSetting")
				{
					var parameters = x.GetParameters();
					return parameters[0].ParameterType.Name == "T" && parameters[1].ParameterType.Equals(typeof(int));
				}
				return false;
			});

			var settings = _data.Settings();
			foreach (var setting in settings)
			{
				Type settingType = setting.GetType();
				Type settingServiceType = typeof(ISettingService);

				var settingService = this.SettingService;
				if (settingService != null)
				{
					var genericMethod = method.MakeGenericMethod(settingType);
					int storeId = (settingType.Equals(typeof(ThemeSettings)) ? _defaultSiteId : 0);

					genericMethod.Invoke(settingService, new object[] { setting, storeId });
				}
			}

			_ctx.SaveChanges();
        }
 
		private void PopulateCategories()
        {
            var channels = _data.Channels();
            SaveRange(channels.Where(x => x != null));

            var categoriesFirstLevel = _data.CategoriesFirstLevel();
			SaveRange(categoriesFirstLevel);
            //search engine names
            categoriesFirstLevel.Each(x =>
            {
                Save(new UrlRecord()
                {
                    EntityId = x.Id,
                    EntityName = "ArticleCategory",
                    LanguageId = 0,
                    Slug = ValidateSeName(x, x.Alias),
                    IsActive = true
                });
            });

            var categoriesSecondLevel = _data.CategoriesSecondLevel();
            SaveRange(categoriesSecondLevel);
            //search engine names
            categoriesSecondLevel.Each(x =>
            {
                Save(new UrlRecord()
                {
                    EntityId = x.Id,
                    EntityName = "ArticleCategory",
                    LanguageId = 0,
                    Slug = ValidateSeName(x, x.Alias),
                    IsActive = true
                });
            });
        }
 

		private void PopulateArticles()
        {
            var products = _data.Articles();
			SaveRange(products);
            //search engine names
            products.Each(x =>
            {
                Save(new UrlRecord()
                {
                    EntityId = x.Id,
                    EntityName = "Article",
                    LanguageId = 0,
					Slug = ValidateSeName(x, x.Title),
                    IsActive = true
                });
            });

			 
        }

     

        private void AddArticleTag(Article product, string tag)
        {
			var productTag = _ctx.Set<ArticleTag>().FirstOrDefault(pt => pt.Name == tag);
            if (productTag == null)
            {
                productTag = new ArticleTag()
                {
                    Name = tag
                };
            }
			product.ArticleTags.Add(productTag);
			Save(product);
        }

		private void MovePictures()
		{
			if (!_config.StoreMediaInDB)
			{
				// All pictures have initially been stored in the DB.
				// Move the binaries to disk
				var pics = _ctx.Set<Picture>().ToList();
				foreach (var pic in pics)
				{
					this.PictureService.UpdatePicture(pic.Id, pic.PictureBinary, pic.MimeType, pic.SeoFilename, pic.IsNew, false);
				}
				_ctx.SaveChanges();
			}
		}

        #endregion

        #region Properties

        protected SeedDataConfiguration Configuration
        {
            get
            {
                return _config;
            }
        }

		protected DefaultObjectContext DataContext
		{
			get
			{
				return _ctx;
			}
		}


		protected ISettingService SettingService
		{
			get
			{
				if (_settingService == null)
				{
					var rs = new EfRepository<Setting>(_ctx);
					rs.AutoCommitEnabled = false;

					_settingService = new SettingService(NullCache.Instance, NullEventPublisher.Instance, rs);
				}

				return _settingService;
			}
		}

		protected IGenericAttributeService GenericAttributeService
		{
			get
			{
				if (_gaService == null)
				{
					var rs = new EfRepository<GenericAttribute>(_ctx);
					rs.AutoCommitEnabled = false;

			

					_gaService = new GenericAttributeService(NullCache.Instance, rs, NullEventPublisher.Instance);
				}

				return _gaService;
			}
		}

		protected IPictureService PictureService
		{
			get
			{
				if (_pictureService == null)
				{
					var rs = new EfRepository<Picture>(_ctx);
					rs.AutoCommitEnabled = false;

                    var rsMap = new EfRepository<ArticleAlbum>(_ctx);
					rs.AutoCommitEnabled = false;
					
					var mediaSettings = new MediaSettings();
					var webHelper = new WebHelper(null);

					_pictureService = new PictureService(
						rs, 
						rsMap,
						this.SettingService,
						webHelper,
						NullLogger.Instance,
						NullEventPublisher.Instance,
						mediaSettings,
						new ImageResizerService(),
						new ImageCache(mediaSettings, webHelper, null, null),
						new Notifier());
				}

				return _pictureService;
			}
		}

		protected ILocalizationService LocalizationService
		{
			get
			{
				if (_locService == null)
				{
					var rsLanguage = new EfRepository<Language>(_ctx);
					rsLanguage.AutoCommitEnabled = false;

					var rsResources = new EfRepository<LocaleStringResource>(_ctx);
					rsResources.AutoCommitEnabled = false;

					var storeMappingService = new SiteMappingService(NullCache.Instance, null, null, null);
					var storeService = new SiteService(NullCache.Instance, new EfRepository<Site>(_ctx), NullEventPublisher.Instance);
					var storeContext = new WebSiteContext(storeService, new WebHelper(null), null);

					var locSettings = new LocalizationSettings();

					var languageService = new LanguageService(
						NullCache.Instance, 
						rsLanguage,
						storeService,
						storeContext,
						NullEventPublisher.Instance
						);

					_locService = new LocalizationService(
						NullCache.Instance,
						NullLogger.Instance,
						null /* IWorkContext: not needed during install */,
						rsResources,
						languageService,
						NullEventPublisher.Instance);
				}

				return _locService;
			}
		}

        #endregion Properties

        #region Methods

        public virtual void Seed(DefaultObjectContext context)
        {
			Guard.ArgumentNotNull(() => context);

			_ctx = context;
			_data.Initialize(_ctx);
			
			_ctx.Configuration.AutoDetectChangesEnabled = false;
			_ctx.Configuration.ValidateOnSaveEnabled = false;
			_ctx.HooksEnabled = false;

			_config.ProgressMessageCallback("Progress.CreatingRequiredData");

            // special mandatory (non-visible) settings
			_ctx.MigrateSettings(x =>
			{
				x.Add("Media.Images.SiteInDB", _config.StoreMediaInDB);
			});
			Populate("PopulatePictures", _data.Pictures());
			Populate("PopulateSites", PopulateSites);
            Populate("PopulateArticleTemplates", _data.ModelTemplates());
			Populate("InstallLanguages", () => PopulateLanguage(_config.Language));
			Populate("PopulateMeasureDimensions", _data.MeasureDimensions());
			Populate("PopulateMeasureWeights", _data.MeasureWeights());
			Populate("PopulateTaxCategories", PopulateTaxCategories);
			Populate("PopulateCountriesAndStates", PopulateCountriesAndStates);
			Populate("PopulateDeliveryTimes", _data.DeliveryTimes());
			Populate("PopulateUsersAndUsers", () => PopulateUsersAndUsers(_config.DefaultUserName, _config.DefaultUserPassword));
			Populate("PopulateEmailAccounts", _data.EmailAccounts());
			Populate("PopulateMessageTemplates", _data.MessageTemplates());
			Populate("PopulateTopics", _data.Topics());
			Populate("PopulateSettings", PopulateSettings);
			Populate("PopulateLocaleResources", PopulateLocaleResources);
			Populate("PopulateActivityLogTypes", _data.ActivityLogTypes());
			Populate("PopulateUsersAndUsers", () => HashDefaultUserPassword(_config.DefaultUserName, _config.DefaultUserPassword));
			Populate("PopulateScheduleTasks", _data.ScheduleTasks());

            if (_config.SeedSampleData)
            {
				_config.ProgressMessageCallback("Progress.CreatingSampleData");
				Populate("PopulateCategories", PopulateCategories);
				Populate("PopulateArticles", PopulateArticles);
				Populate("PopulateForumsGroups", _data.ForumGroups());
				Populate("PopulateForums", _data.Forums());;
				Populate("PopulatePolls", _data.Polls());
            }

			Populate("MovePictures", MovePictures);
        }

		public bool RollbackOnFailure
		{
			get { return false; }
		}

        #endregion

		#region Utils

		private void SetModified<TEntity>(TEntity entity) 
			where TEntity : BaseEntity
		{
			_ctx.Set<TEntity>().Attach(entity);
			_ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
		}

		private string ValidateSeName<TEntity>(TEntity entity, string name)
			where TEntity : BaseEntity, ISlugSupported
		{
			var seoSettings = new SeoSettings { LoadAllUrlAliasesOnStartup = false };
			
			if (_urlRecordService == null)
			{
				_urlRecordService = new UrlRecordService(NullCache.Instance, new EfRepository<UrlRecord>(_ctx) { AutoCommitEnabled = false });
			}

			return entity.ValidateSeName<TEntity>("", name, true, _urlRecordService, new SeoSettings());
		}

		private void Populate<TEntity>(string stage, IEnumerable<TEntity> entities) 
			where TEntity : BaseEntity
		{
			try
			{
				SaveRange(entities);
			}
			catch (Exception ex)
			{
				throw new SeedDataException(stage, ex);
			}
		}

		private void Populate(string stage, Action populateAction)
		{
			try
			{
				populateAction();
			}
			catch (Exception ex)
			{
				throw new SeedDataException(stage, ex);
			}
		}

		private void Save<TEntity>(TEntity entity) where TEntity : BaseEntity
		{
			_ctx.Set<TEntity>().Add(entity);
			_ctx.SaveChanges();
		}

		private void SaveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
		{
			_ctx.Set<TEntity>().AddRange(entities);
			_ctx.SaveChanges();
		}

		#endregion

	}
        
} 
