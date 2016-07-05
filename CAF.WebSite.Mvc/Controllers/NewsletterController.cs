using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Models.Newsletter;
using CAF.Infrastructure.Core;
using System;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Controllers
{
    public partial class NewsletterController : PublicControllerBase
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkflowMessageService _workflowMessageService;
		private readonly ISiteContext _siteContext;

        private readonly UserSettings _userSettings;

        public NewsletterController(ILocalizationService localizationService,
            IWorkContext workContext, INewsLetterSubscriptionService newsLetterSubscriptionService,
            IWorkflowMessageService workflowMessageService, UserSettings userSettings,
			ISiteContext siteContext)
        {
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._workflowMessageService = workflowMessageService;
            this._userSettings = userSettings;
			this._siteContext = siteContext;
        }
        /// <summary>
        /// 通讯箱
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult NewsletterBox()
        {
            if (_userSettings.HideNewsletterBlock)
                return Content("");

            return PartialView(new NewsletterBoxModel());
        }
        /// <summary>
        ///  订阅
        /// </summary>
        /// <param name="subscribe"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Subscribe(bool subscribe, string email)
        {
            string result;
            bool success = false;

			if (!email.IsEmail())
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
            else
            {
                //subscribe/unsubscribe
                email = email.Trim();

                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(email, _siteContext.CurrentSite.Id);
                if (subscription != null)
                {
                    if (subscribe)
                    {
                        if (!subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                    }
                    else
                    {
                        if (subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                    }
                }
                else if (subscribe)
                {
                    subscription = new NewsLetterSubscription()
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = email,
                        Active = false,
                        CreatedOnUtc = DateTime.UtcNow,
						SiteId = _siteContext.CurrentSite.Id
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);

                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                }
                success = true;
            }

            return Json(new
            {
                Success = success,
                Result = result,
            });
        }
        /// <summary>
        /// 订阅激活
        /// </summary>
        /// <param name="token"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public ActionResult SubscriptionActivation(Guid token, bool active)
        {	
			var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuid(token);
            if (subscription == null)
				return HttpNotFound();

            var model = new SubscriptionActivationModel();

            if (active)
            {
                subscription.Active = active;
                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
            }
            else
                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            if (active)
                model.Result = _localizationService.GetResource("Newsletter.ResultActivated");
            else
                model.Result = _localizationService.GetResource("Newsletter.ResultDeactivated");

            return View(model);
        }
    }
}
