using CAF.Infrastructure.Core.Collections;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.WebSite.Mvc.Admin.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class MessageTemplateController : AdminControllerBase
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;
		private readonly ISiteService _siteService;
		private readonly ISiteMappingService _siteMappingService;
        private readonly EmailAccountSettings _emailAccountSettings;
        #endregion Fields

        #region Constructors

        public MessageTemplateController(IMessageTemplateService messageTemplateService, 
            IEmailAccountService emailAccountService, ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, IMessageTokenProvider messageTokenProvider,
			IPermissionService permissionService, ISiteService siteService,
			ISiteMappingService siteMappingService,
			EmailAccountSettings emailAccountSettings)
        {
            this._messageTemplateService = messageTemplateService;
            this._emailAccountService = emailAccountService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._permissionService = permissionService;
			this._siteService = siteService;
			this._siteMappingService = siteMappingService;
            this._emailAccountSettings = emailAccountSettings;
        }

        private void FillTokensTree(TreeNode<string> root, string[] tokens)
        {
            root.Clear();

            //Array.Sort(tokens);

            for (int i = 0; i < tokens.Length; i++)
            {
                // remove '%' from '%Order.ID%''
                string token = tokens[i].Trim('%');
                // split 'Order.ID' to [ Order, ID ] parts
                var parts = token.Split('.');

                var node = root;
                // iterate parts
                foreach (var part in parts)
                {
                    var found = node.Find(part);
                    if (found == null)
                    {
                        node = node.Append(part);
                    }
                    else
                    {
                        node = found;
                    }
                }
            }
        }

        #endregion
        
        #region Utilities

        [NonAction]
        public void UpdateLocales(MessageTemplate mt, MessageTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.BccEmailAddresses,
                                                           localized.BccEmailAddresses,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Subject,
                                                           localized.Subject,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.EmailAccountId,
                                                           localized.EmailAccountId,
                                                           localized.LanguageId);
            }
        }


		[NonAction]
		private void PrepareSitesMappingModel(MessageTemplateModel model, MessageTemplate messageTemplate, bool excludeProperties)
		{
			if (model == null)
				throw new ArgumentNullException("model");

			model.AvailableSites = _siteService
				.GetAllSites()
				.Select(s => s.ToModel())
				.ToList();
			if (!excludeProperties)
			{
				if (messageTemplate != null)
				{
                    model.SelectedSiteIds = _siteMappingService.GetSitesIdsWithAccess(messageTemplate);
				}
				else
				{
					model.SelectedSiteIds = new int[0];
				}
			}
		}

		[NonAction]
		protected void SaveSiteMappings(MessageTemplate messageTemplate, MessageTemplateModel model)
		{
			var existingSiteMappings = _siteMappingService.GetSiteMappings(messageTemplate);
			var allSites = _siteService.GetAllSites();
			foreach (var site in allSites)
			{
				if (model.SelectedSiteIds != null && model.SelectedSiteIds.Contains(site.Id))
				{
					//new role
					if (existingSiteMappings.Where(sm => sm.SiteId == site.Id).Count() == 0)
						_siteMappingService.InsertSiteMapping(messageTemplate, site.Id);
				}
				else
				{
					//removed role
					var siteMappingToDelete = existingSiteMappings.Where(sm => sm.SiteId == site.Id).FirstOrDefault();
					if (siteMappingToDelete != null)
						_siteMappingService.DeleteSiteMapping(siteMappingToDelete);
				}
			}
		}
        
        #endregion
        
        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

			var model = new MessageTemplateListModel();
			//sites
			model.AvailableSites.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
			foreach (var s in _siteService.GetAllSites())
				model.AvailableSites.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });

			return View(model);
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam, MessageTemplateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

			var messageTemplates = _messageTemplateService.GetAllMessageTemplates(model.SearchSiteId);
            var total = messageTemplates.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = messageTemplates.Select(x => x.ToModel()).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");


            var model = messageTemplate.ToModel();

            FillTokensTree(model.TokensTree, _messageTokenProvider.GetListOfAllowedTokens());

            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(ea.ToModel());
			//Site
			PrepareSitesMappingModel(model, messageTemplate, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.BccEmailAddresses = messageTemplate.GetLocalized(x => x.BccEmailAddresses, languageId, false, false);
                locale.Subject = messageTemplate.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = messageTemplate.GetLocalized(x => x.Body, languageId, false, false);

                var emailAccountId = messageTemplate.GetLocalized(x => x.EmailAccountId, languageId, false, false);
                locale.EmailAccountId = emailAccountId > 0 ? emailAccountId : _emailAccountSettings.DefaultEmailAccountId;
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
		[FormValueRequired("save", "save-continue")]
        public ActionResult Edit(MessageTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                messageTemplate = model.ToEntity(messageTemplate);
                _messageTemplateService.UpdateMessageTemplate(messageTemplate);

                //Sites
                _siteMappingService.SaveSiteMappings<MessageTemplate>(messageTemplate, model.SelectedSiteIds);

                //locales
                UpdateLocales(messageTemplate, model);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Updated"));
                return continueEditing ? RedirectToAction("Edit", messageTemplate.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            FillTokensTree(model.TokensTree, _messageTokenProvider.GetListOfAllowedTokens());

            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(ea.ToModel());

            //Site
            PrepareSitesMappingModel(model, messageTemplate, true);
            return View(model);
        }

		[HttpPost]
		public ActionResult Delete(int id)
		{
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
				return AccessDeniedView();

			var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
			if (messageTemplate == null)
				//No message template found with the specified id
				return RedirectToAction("List");

			_messageTemplateService.DeleteMessageTemplate(messageTemplate);

			NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Deleted"));
			return RedirectToAction("List");
		}

		[HttpPost, ActionName("Edit")]
		[FormValueRequired("message-template-copy")]
		public ActionResult CopyTemplate(MessageTemplateModel model)
		{
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
				return AccessDeniedView();

			var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
			if (messageTemplate == null)
				//No message template found with the specified id
				return RedirectToAction("List");

			try
			{
				var newMessageTemplate = _messageTemplateService.CopyMessageTemplate(messageTemplate);
				NotifySuccess("The message template has been copied successfully");
				return RedirectToAction("Edit", new { id = newMessageTemplate.Id });
			}
			catch (Exception exc)
			{
				NotifyError(exc.Message);
				return RedirectToAction("Edit", new { id = model.Id });
			}
		}

        #endregion
    }
}
