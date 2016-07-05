using CAF.Infrastructure.Core.Domain.Messages;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Email;

using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using System.Web;
using CAF.Infrastructure.Core.Domain.Common;
//using CAF.Infrastructure.Core.Domain.Shop.Orders;
//using CAF.Infrastructure.Core.Domain.Shop.Shipping;
//using CAF.Infrastructure.Core.Domain.Shop.Catalog;
using CAF.Infrastructure.Core.Domain.Sites;



namespace CAF.WebSite.Application.Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILanguageService _languageService;
        private readonly ITokenizer _tokenizer;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ISiteService _siteService;
        private readonly ISiteContext _siteContext;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly HttpRequestBase _httpRequest;

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService, ILanguageService languageService,
            ITokenizer tokenizer, IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider,
            ISiteService siteService,
            ISiteContext siteContext,
            EmailAccountSettings emailAccountSettings,
            IEventPublisher eventPublisher,
            IWorkContext workContext,
            HttpRequestBase httpRequest)
        {
            this._messageTemplateService = messageTemplateService;
            this._queuedEmailService = queuedEmailService;
            this._languageService = languageService;
            this._tokenizer = tokenizer;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;
            this._siteService = siteService;
            this._siteContext = siteContext;
            this._emailAccountSettings = emailAccountSettings;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
            _httpRequest = httpRequest;
        }

        #endregion

        #region Utilities

        protected int SendNotification(
            MessageTemplate messageTemplate,
            EmailAccount emailAccount,
            int languageId,
            IEnumerable<Token> tokens,
            string toEmailAddress,
            string toName,
            string replyTo = null,
            string replyToName = null)
        {
            // retrieve localized message template data
            var bcc = messageTemplate.GetLocalized((mt) => mt.BccEmailAddresses, languageId);
            var subject = messageTemplate.GetLocalized((mt) => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized((mt) => mt.Body, languageId);

            // Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            bodyReplaced = WebHelper.MakeAllUrlsAbsolute(bodyReplaced, _httpRequest);

            var email = new QueuedEmail()
            {
                Priority = 5,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                CC = string.Empty,
                Bcc = bcc,
                ReplyTo = replyTo,
                ReplyToName = replyToName,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        protected MessageTemplate GetLocalizedActiveMessageTemplate(string messageTemplateName,
            int languageId, int siteId)
        {
            //TODO remove languageId parameter
            var messageTemplate = _messageTemplateService.GetMessageTemplateByName(messageTemplateName, siteId);

            //no template found
            if (messageTemplate == null)
                return null;

            //ensure it's active
            var isActive = messageTemplate.IsActive;
            if (!isActive)
                return null;

            return messageTemplate;
        }

        protected EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccounId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccounId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();

            return emailAccount;
        }

        private Tuple<string, string> GetReplyToEmail(User user)
        {
            if (user == null || user.Email.IsEmpty())
                return new Tuple<string, string>(null, null);

            string email = user.Email;
            string name = GetDisplayNameForUser(user);

            return new Tuple<string, string>(email, name);
        }

        private string GetDisplayNameForUser(User user)
        {
            if (user == null)
                return string.Empty;

            Func<Address, string> getName = (address) =>
            {
                if (address == null)
                    return null;

                string result = string.Empty;
                if (address.FirstName.HasValue() || address.LastName.HasValue())
                {
                    result = string.Format("{0} {1}", address.FirstName, address.LastName).Trim();
                }

                if (address.Company.HasValue())
                {
                    result = string.Concat(result, result.HasValue() ? ", " : "", address.Company);
                }

                return result;
            };

            string name = getName(user.BillingAddress);
            if (name.IsEmpty())
            {
                name = getName(user.ShippingAddress);
            }
            if (name.IsEmpty())
            {
                name = getName(user.Addresses.FirstOrDefault());
            }

            name = name.TrimSafe().NullEmpty();

            return name ?? user.UserName.EmptyNull();
        }

        protected int EnsureLanguageIsActive(int languageId, int siteId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified site
                language = _languageService.GetAllLanguages(siteId: siteId).FirstOrDefault();
            }
            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");
            return language.Id;
        }

        #endregion

        #region Methods

        #region User workflow

        /// <summary>
        /// Sends 'New user' notification message to a site owner
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserRegisteredNotificationMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var site = _siteContext.CurrentSite;
            languageId = EnsureLanguageIsActive(languageId, site.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewUser.Notification", languageId, site.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddUserTokens(tokens, user);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            // use user email as reply address
            var replyTo = GetReplyToEmail(user);

            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName,
                replyTo.Item1, replyTo.Item2);
        }

        /// <summary>
        /// Sends a welcome message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserWelcomeMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var site = _siteContext.CurrentSite;
            languageId = EnsureLanguageIsActive(languageId, site.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("User.WelcomeMessage", languageId, site.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddUserTokens(tokens, user);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = user.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserEmailValidationMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var site = _siteContext.CurrentSite;
            languageId = EnsureLanguageIsActive(languageId, site.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("User.EmailValidationMessage", languageId, site.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddUserTokens(tokens, user);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = user.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends password recovery message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserPasswordRecoveryMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var site = _siteContext.CurrentSite;
            languageId = EnsureLanguageIsActive(languageId, site.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("User.PasswordRecovery", languageId, site.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddUserTokens(tokens, user);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = user.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        #endregion

        #region Order workflow

        ///// <summary>
        ///// Sends an order placed notification to a site owner
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendOrderPlacedSiteOwnerNotification(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException("order");

        //    var site = _siteService.GetSiteById(order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.SiteOwnerNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
        //    _messageTokenProvider.AddUserTokens(tokens, order.User);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = emailAccount.Email;
        //    var toName = emailAccount.DisplayName;

        //    // use buyer's email as reply address
        //    var replyToEmail = order.BillingAddress.Email;
        //    var replyToName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
        //    if (order.BillingAddress.Company.HasValue())
        //    {
        //        replyToName += ", " + order.BillingAddress.Company;
        //    }

        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName,
        //        replyToEmail, replyToName);
        //}

        ///// <summary>
        ///// Sends an order placed notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendOrderPlacedUserNotification(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException("order");

        //    var site = _siteService.GetSiteById(order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.UserNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
        //    _messageTokenProvider.AddUserTokens(tokens, order.User);

        //    _messageTokenProvider.AddCompanyTokens(tokens);
        //    _messageTokenProvider.AddBankConnectionTokens(tokens);
        //    _messageTokenProvider.AddContactDataTokens(tokens);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = order.BillingAddress.Email;
        //    var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        ///// <summary>
        ///// Sends a shipment sent notification to a user
        ///// </summary>
        ///// <param name="shipment">Shipment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendShipmentSentUserNotification(Shipment shipment, int languageId)
        //{
        //    if (shipment == null)
        //        throw new ArgumentNullException("shipment");

        //    var order = shipment.Order;
        //    if (order == null)
        //        throw new Exception("Order cannot be loaded");

        //    var site = _siteService.GetSiteById(order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("ShipmentSent.UserNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
        //    _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
        //    _messageTokenProvider.AddUserTokens(tokens, shipment.Order.User);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = order.BillingAddress.Email;
        //    var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        ///// <summary>
        ///// Sends a shipment delivered notification to a user
        ///// </summary>
        ///// <param name="shipment">Shipment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendShipmentDeliveredUserNotification(Shipment shipment, int languageId)
        //{
        //    if (shipment == null)
        //        throw new ArgumentNullException("shipment");

        //    var order = shipment.Order;
        //    if (order == null)
        //        throw new Exception("Order cannot be loaded");

        //    var site = _siteService.GetSiteById(order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("ShipmentDelivered.UserNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
        //    _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
        //    _messageTokenProvider.AddUserTokens(tokens, shipment.Order.User);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = order.BillingAddress.Email;
        //    var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        ///// <summary>
        ///// Sends an order completed notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendOrderCompletedUserNotification(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException("order");

        //    var site = _siteService.GetSiteById(order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("OrderCompleted.UserNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
        //    _messageTokenProvider.AddUserTokens(tokens, order.User);

        //    _messageTokenProvider.AddCompanyTokens(tokens);
        //    _messageTokenProvider.AddBankConnectionTokens(tokens);
        //    _messageTokenProvider.AddContactDataTokens(tokens);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = order.BillingAddress.Email;
        //    var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        ///// <summary>
        ///// Sends an order cancelled notification to a user
        ///// </summary>
        ///// <param name="order">Order instance</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendOrderCancelledUserNotification(Order order, int languageId)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException("order");

        //    var site = _siteService.GetSiteById(order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("OrderCancelled.UserNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
        //    _messageTokenProvider.AddUserTokens(tokens, order.User);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = order.BillingAddress.Email;
        //    var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        ///// <summary>
        ///// Sends a new order note added notification to a user
        ///// </summary>
        ///// <param name="orderNote">Order note</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendNewOrderNoteAddedUserNotification(OrderNote orderNote, int languageId)
        //{
        //    if (orderNote == null)
        //        throw new ArgumentNullException("orderNote");

        //    var order = orderNote.Order;

        //    var site = _siteService.GetSiteById(order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("User.NewOrderNote", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddOrderNoteTokens(tokens, orderNote);
        //    _messageTokenProvider.AddOrderTokens(tokens, orderNote.Order, languageId);
        //    _messageTokenProvider.AddUserTokens(tokens, orderNote.Order.User);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = order.BillingAddress.Email;
        //    var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        ///// <summary>
        ///// Sends a "Recurring payment cancelled" notification to a site owner
        ///// </summary>
        ///// <param name="recurringPayment">Recurring payment</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendRecurringPaymentCancelledSiteOwnerNotification(RecurringPayment recurringPayment, int languageId)
        //{
        //    if (recurringPayment == null)
        //        throw new ArgumentNullException("recurringPayment");

        //    var site = _siteService.GetSiteById(recurringPayment.InitialOrder.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("RecurringPaymentCancelled.SiteOwnerNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
        //    _messageTokenProvider.AddUserTokens(tokens, recurringPayment.InitialOrder.User);
        //    _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = emailAccount.Email;
        //    var toName = emailAccount.DisplayName;

        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            var site = _siteContext.CurrentSite;
            languageId = EnsureLanguageIsActive(languageId, site.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.ActivationMessage", languageId, site.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = subscription.Email;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            var site = _siteContext.CurrentSite;
            languageId = EnsureLanguageIsActive(languageId, site.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.DeactivationMessage", languageId, site.Id);
            if (messageTemplate == null)
                return 0;

            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, _siteContext.CurrentSite);
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = subscription.Email;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        #endregion

        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="userEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        //public virtual int SendProductEmailAFriendMessage(User user, int languageId,
        //    Product product, string userEmail, string friendsEmail, string personalMessage)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException("user");

        //    if (product == null)
        //        throw new ArgumentNullException("product");

        //    var site = _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("Service.EmailAFriend", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddUserTokens(tokens, user);
        //    _messageTokenProvider.AddProductTokens(tokens, product, languageId);
        //    tokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
        //    tokens.Add(new Token("EmailAFriend.Email", userEmail));

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = friendsEmail;
        //    var toName = "";
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        //public virtual int SendProductQuestionMessage(User user, int languageId, Product product,
        //    string senderEmail, string senderName, string senderPhone, string question)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException("user");

        //    if (user.IsSystemAccount)
        //        return 0;

        //    if (product == null)
        //        throw new ArgumentNullException("product");

        //    var site = _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("Product.AskQuestion", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddUserTokens(tokens, user);
        //    _messageTokenProvider.AddProductTokens(tokens, product, languageId);

        //    tokens.Add(new Token("ProductQuestion.Message", question, true));
        //    tokens.Add(new Token("ProductQuestion.SenderEmail", senderEmail));
        //    tokens.Add(new Token("ProductQuestion.SenderName", senderName));
        //    tokens.Add(new Token("ProductQuestion.SenderPhone", senderPhone));

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = emailAccount.Email;
        //    var toName = emailAccount.DisplayName;

        //    return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, senderEmail, senderName);
        //}

        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="userEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendWishlistEmailAFriendMessage(User user, int languageId,
             string userEmail, string friendsEmail, string personalMessage)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var site = _siteContext.CurrentSite;
            languageId = EnsureLanguageIsActive(languageId, site.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Wishlist.EmailAFriend", languageId, site.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddUserTokens(tokens, user);
            tokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
            tokens.Add(new Token("Wishlist.Email", userEmail));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = friendsEmail;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        #endregion

        #region Return requests

        ///// <summary>
        ///// Sends 'New Return Request' message to a site owner
        ///// </summary>
        ///// <param name="returnRequest">Return request</param>
        ///// <param name="orderItem">Order item</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendNewReturnRequestSiteOwnerNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        //{
        //    if (returnRequest == null)
        //        throw new ArgumentNullException("returnRequest");

        //    var site = _siteService.GetSiteById(orderItem.Order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("NewReturnRequest.SiteOwnerNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddUserTokens(tokens, returnRequest.User);
        //    _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, orderItem);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = emailAccount.Email;
        //    var toName = emailAccount.DisplayName;

        //    // use user email as reply address
        //    var replyTo = GetReplyToEmail(returnRequest.User);

        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName,
        //        replyTo.Item1, replyTo.Item2);
        //}

        ///// <summary>
        ///// Sends 'Return Request status changed' message to a user
        ///// </summary>
        ///// <param name="returnRequest">Return request</param>
        ///// <param name="orderItem">Order item</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendReturnRequestStatusChangedUserNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        //{
        //    if (returnRequest == null)
        //        throw new ArgumentNullException("returnRequest");

        //    var site = _siteService.GetSiteById(orderItem.Order.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("ReturnRequestStatusChanged.UserNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddUserTokens(tokens, returnRequest.User);
        //    _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, orderItem);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = returnRequest.User.FindEmail();
        //    var toName = returnRequest.User.GetFullName();

        //    if (toEmail.IsEmpty())
        //        return 0;

        //    return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        //}

        #endregion

        #region Forum Notifications

        /// <summary>
        /// Sends a forum subscription message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumTopicMessage(User user,
            ForumTopic forumTopic, Forum forum, int languageId)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var site = _siteContext.CurrentSite;

            var messageTemplate = GetLocalizedActiveMessageTemplate("Forums.NewForumTopic", languageId, site.Id);
            if (messageTemplate == null)
            {
                return 0;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddUserTokens(tokens, user);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
            _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = user.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends a forum subscription message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumPostMessage(User user,
            ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex, int languageId)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var site = _siteContext.CurrentSite;

            var messageTemplate = GetLocalizedActiveMessageTemplate("Forums.NewForumPost", languageId, site.Id);
            if (messageTemplate == null)
            {
                return 0;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddForumPostTokens(tokens, forumPost);
            _messageTokenProvider.AddUserTokens(tokens, user);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumPost.ForumTopic,
                friendlyForumTopicPageIndex, forumPost.Id);
            _messageTokenProvider.AddForumTokens(tokens, forumPost.ForumTopic.Forum);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = user.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendPrivateMessageNotification(User user, PrivateMessage privateMessage, int languageId)
        {
            if (privateMessage == null)
            {
                throw new ArgumentNullException("privateMessage");
            }

            var site = _siteService.GetSiteById(privateMessage.SiteId) ?? _siteContext.CurrentSite;

            var messageTemplate = GetLocalizedActiveMessageTemplate("User.NewPM", languageId, site.Id);
            if (messageTemplate == null)
            {
                return 0;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddUserTokens(tokens, user);
            _messageTokenProvider.AddPrivateMessageTokens(tokens, privateMessage);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = privateMessage.ToUser.Email;
            var toName = privateMessage.ToUser.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        #endregion

        #region Misc

        public virtual int SendGenericMessage(string messageTemplateName, Action<GenericMessageContext> cfg)
        {
            Guard.ArgumentNotNull(() => cfg);
            Guard.ArgumentNotEmpty(() => messageTemplateName);

            var ctx = new GenericMessageContext();
            ctx.MessagenTokenProvider = _messageTokenProvider;

            cfg(ctx);

            if (!ctx.SiteId.HasValue)
            {
                ctx.SiteId = _siteContext.CurrentSite.Id;
            }

            if (!ctx.LanguageId.HasValue)
            {
                ctx.LanguageId = _workContext.WorkingLanguage.Id;
            }

            if (ctx.User == null)
            {
                ctx.User = _workContext.CurrentUser;
            }

            if (ctx.User.IsSystemAccount)
                return 0;

            _messageTokenProvider.AddUserTokens(ctx.Tokens, ctx.User);
            _messageTokenProvider.AddSiteTokens(ctx.Tokens, _siteService.GetSiteById(ctx.SiteId.Value));

            ctx.LanguageId = EnsureLanguageIsActive(ctx.LanguageId.Value, ctx.SiteId.Value);

            var messageTemplate = GetLocalizedActiveMessageTemplate(messageTemplateName, ctx.LanguageId.Value, ctx.SiteId.Value);
            if (messageTemplate == null)
                return 0;

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, ctx.LanguageId.Value);
            var toEmail = ctx.ToEmail.HasValue() ? ctx.ToEmail : emailAccount.Email;
            var toName = ctx.ToName.HasValue() ? ctx.ToName : emailAccount.DisplayName;

            if (ctx.ReplyToUser && ctx.User != null)
            {
                // use user email as reply address
                var replyTo = GetReplyToEmail(ctx.User);
                ctx.ReplyToEmail = replyTo.Item1;
                ctx.ReplyToName = replyTo.Item2;
            }

            return SendNotification(messageTemplate, emailAccount, ctx.LanguageId.Value, ctx.Tokens, toEmail, toName, ctx.ReplyToEmail, ctx.ReplyToName);
        }

        ///// <summary>
        ///// Sends a gift card notification
        ///// </summary>
        ///// <param name="giftCard">Gift card</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendGiftCardNotification(GiftCard giftCard, int languageId)
        //{
        //    if (giftCard == null)
        //        throw new ArgumentNullException("giftCard");

        //    Site site = null;
        //    var order = giftCard.PurchasedWithOrderItem != null ?
        //        giftCard.PurchasedWithOrderItem.Order :
        //        null;
        //    if (order != null)
        //        site = _siteService.GetSiteById(order.SiteId);
        //    if (site == null)
        //        site = _siteContext.CurrentSite;

        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("GiftCard.Notification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddGiftCardTokens(tokens, giftCard);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = giftCard.RecipientEmail;
        //    var toName = giftCard.RecipientName;
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        ///// <summary>
        ///// Sends a product review notification message to a site owner
        ///// </summary>
        ///// <param name="productReview">Product review</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendProductReviewNotificationMessage(ProductReview productReview,
        //    int languageId)
        //{
        //    if (productReview == null)
        //        throw new ArgumentNullException("productReview");

        //    var site = _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("Product.ProductReview", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddProductReviewTokens(tokens, productReview);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = emailAccount.Email;
        //    var toName = emailAccount.DisplayName;

        //    // use user email as reply address
        //    var replyTo = GetReplyToEmail(productReview.User);

        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName,
        //        replyTo.Item1, replyTo.Item2);
        //}

        ///// <summary>
        ///// Sends a "quantity below" notification to a site owner
        ///// </summary>
        ///// <param name="product">Product</param>
        ///// <param name="languageId">Message language identifier</param>
        ///// <returns>Queued email identifier</returns>
        //public virtual int SendQuantityBelowSiteOwnerNotification(Product product, int languageId)
        //{
        //    if (product == null)
        //        throw new ArgumentNullException("product");

        //    var site = _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("QuantityBelow.SiteOwnerNotification", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddProductTokens(tokens, product, languageId);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = emailAccount.Email;
        //    var toName = emailAccount.DisplayName;
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        /// <summary>
        /// Sends a "new VAT sumitted" notification to a site owner
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewVatSubmittedSiteOwnerNotification(User user,
            string vatName, string vatAddress, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var site = _siteContext.CurrentSite;
            languageId = EnsureLanguageIsActive(languageId, site.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewVATSubmitted.SiteOwnerNotification", languageId, site.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddSiteTokens(tokens, site);
            _messageTokenProvider.AddUserTokens(tokens, user);
            tokens.Add(new Token("VatValidationResult.Name", vatName));
            tokens.Add(new Token("VatValidationResult.Address", vatAddress));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            // use user email as reply address
            var replyTo = GetReplyToEmail(user);

            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName,
                replyTo.Item1, replyTo.Item2);
        }

        /// <summary>
        /// Sends a blog comment notification message to a site owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        //public virtual int SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId)
        //{
        //    if (blogComment == null)
        //        throw new ArgumentNullException("blogComment");

        //    var site = _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("Blog.BlogComment", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddBlogCommentTokens(tokens, blogComment);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = emailAccount.Email;
        //    var toName = emailAccount.DisplayName;

        //    // use user email as reply address
        //    var replyTo = GetReplyToEmail(blogComment.User);

        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName,
        //        replyTo.Item1, replyTo.Item2);
        //}

        /// <summary>
        /// Sends a news comment notification message to a site owner
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        //public virtual int SendNewsCommentNotificationMessage(NewsComment newsComment, int languageId)
        //{
        //    if (newsComment == null)
        //        throw new ArgumentNullException("newsComment");

        //    var site = _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("News.NewsComment", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddNewsCommentTokens(tokens, newsComment);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var toEmail = emailAccount.Email;
        //    var toName = emailAccount.DisplayName;

        //    // use user email as sender/reply address
        //    var replyTo = GetReplyToEmail(newsComment.User);

        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName,
        //        replyTo.Item1, replyTo.Item2);
        //}

        /// <summary>
        /// Sends a 'Back in stock' notification message to a user
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        //public virtual int SendBackInStockNotification(BackInStockSubscription subscription, int languageId)
        //{
        //    if (subscription == null)
        //        throw new ArgumentNullException("subscription");

        //    var site = _siteService.GetSiteById(subscription.SiteId) ?? _siteContext.CurrentSite;
        //    languageId = EnsureLanguageIsActive(languageId, site.Id);

        //    var messageTemplate = GetLocalizedActiveMessageTemplate("User.BackInStock", languageId, site.Id);
        //    if (messageTemplate == null)
        //        return 0;

        //    //tokens
        //    var tokens = new List<Token>();
        //    _messageTokenProvider.AddSiteTokens(tokens, site);
        //    _messageTokenProvider.AddUserTokens(tokens, subscription.User);
        //    _messageTokenProvider.AddBackInStockTokens(tokens, subscription);

        //    //event notification
        //    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

        //    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
        //    var user = subscription.User;
        //    var toEmail = user.Email;
        //    var toName = user.GetFullName();
        //    return SendNotification(messageTemplate, emailAccount,
        //        languageId, tokens,
        //        toEmail, toName);
        //}

        #endregion

        #endregion
    }
}
