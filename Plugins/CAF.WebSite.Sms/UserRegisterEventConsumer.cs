
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Domain.Users;
using System;


namespace CAF.WebSite.Sms
{
    public class UserRegisterEventConsumer : IConsumer<UserRegisterEvent>
    {
        private readonly SmsSettings _smsSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly ISettingService _settingService;	// codehint: sm-add

        public UserRegisterEventConsumer(SmsSettings smsSettings,
            IPluginFinder pluginFinder,
            IDbContext dbContext,
            ISiteContext siteContext,
            IWorkContext workContext,
            ISettingService settingService)
        {
            this._smsSettings = smsSettings;
            this._pluginFinder = pluginFinder;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._settingService = settingService;	// codehint: sm-add
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(UserRegisterEvent eventMessage)
        {
            //is enabled?
            if (!_smsSettings.Enabled)
                return;

            //is plugin installed?
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("CAF.WebSite.Sms");
            if (pluginDescriptor == null)
                return;

            if (!(_siteContext.CurrentSite.Id == 0 ||
                _settingService.GetSettingByKey<string>(pluginDescriptor.GetSettingKey("LimitedToSites")).ToIntArrayContains(_siteContext.CurrentSite.Id, true)))
                return;

            var plugin = pluginDescriptor.Instance() as SmsProvider;
            if (plugin == null)
                return;

            var user = eventMessage.User;
            //send SMS
            if (plugin.SendSms(String.Format("'{0}'已注册成功，欢迎您的到来.", user.UserName)))
            {
                //记录信息
                //order.OrderNotes.Add(new OrderNote()
                //{
                //    Note = "\"Order placed\" SMS alert (to site owner) has been sent",
                //    DisplayToCustomer = false,
                //    CreatedOnUtc = DateTime.UtcNow
                //});
                //_orderService.UpdateOrder(order);
            }
        }
    }
}