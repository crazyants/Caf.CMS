using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Security
{
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "控制台", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord AllowUserImpersonation = new PermissionRecord { Name = "运行模拟用户", SystemName = "AllowUserImpersonation", Category = "Users" };
        public static readonly PermissionRecord ManageCatalog = new PermissionRecord { Name = "栏目管理", SystemName = "ManageCatalog", Category = "Catalog" };
        public static readonly PermissionRecord ManageChannel = new PermissionRecord { Name = "模型管理", SystemName = "ManageChannel", Category = "Catalog" };
        public static readonly PermissionRecord ManageUsers = new PermissionRecord { Name = "用户管理", SystemName = "ManageUsers", Category = "Users" };
        public static readonly PermissionRecord ManageUserRoles = new PermissionRecord { Name = "用户角色", SystemName = "ManageUserRoles", Category = "Users" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "系统配置", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageSites = new PermissionRecord { Name = "网站管理", SystemName = "ManageSites", Category = "Configuration" };
        public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "访问控制", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "语言管理", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "国家管理", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageArticles = new PermissionRecord { Name = "内容管理", SystemName = "ManageArticles", Category = "Content Management" };
        public static readonly PermissionRecord ManageTopics = new PermissionRecord { Name = "单页管理", SystemName = "ManageTopics", Category = "Content Management" };
        public static readonly PermissionRecord ManageClients = new PermissionRecord { Name = "服务客户管理", SystemName = "ManageClients", Category = "Catalog Management" };
        public static readonly PermissionRecord ManageLinks = new PermissionRecord { Name = "友情链接", SystemName = "ManageLinks", Category = "Content Management" };
        public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "区域块管理", SystemName = "ManageWidgets", Category = "Content Management" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "消息模板", SystemName = "ManageMessageTemplates", Category = "Content Management" };
        public static readonly PermissionRecord ManagePolls = new PermissionRecord { Name = "调查问卷管理", SystemName = "ManagePolls", Category = "Content Management" };
        public static readonly PermissionRecord ManageModelTemplates = new PermissionRecord { Name = "模板页管理", SystemName = "ManageModelTemplate", Category = "Content Management" };
        public static readonly PermissionRecord ManageRegionalContents = new PermissionRecord { Name = "内容块", SystemName = "ManageModelRegionalContent", Category = "Content Management" };
        public static readonly PermissionRecord ManageFeedbacks = new PermissionRecord { Name = "站内留言", SystemName = "ManageFeedback", Category = "Content Management" };


        //codehint: sm-add begin
        public static readonly PermissionRecord ManageCurrencies = new PermissionRecord { Name = "货币管理", SystemName = "ManageCurrencies", Category = "Configuration" };
        public static readonly PermissionRecord ManageQuantityUnits = new PermissionRecord { Name = "量单位管理", SystemName = "ManageQuantityUnits", Category = "Configuration" };
        public static readonly PermissionRecord ManageDeliveryTimes = new PermissionRecord { Name = "交货时间管理", SystemName = "ManageDeliveryTimes", Category = "Configuration" };
        public static readonly PermissionRecord ManageContentSlider = new PermissionRecord { Name = "幻灯片", SystemName = "ManageContentSlider", Category = "Configuration" };
        public static readonly PermissionRecord ManageThemes = new PermissionRecord { Name = "主题管理", SystemName = "ManageThemes", Category = "Configuration" };
        //codehint: sm-add end
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new PermissionRecord { Name = "第三方登录授权管理", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };

        public static readonly PermissionRecord ManageMeasures = new PermissionRecord { Name = "管理措施", SystemName = "ManageMeasures", Category = "Configuration" };
        public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "操作日志", SystemName = "ManageActivityLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "邮箱账号", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "系统日志", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new PermissionRecord { Name = "消息队列", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance = new PermissionRecord { Name = "系统维护", SystemName = "ManageMaintenance", Category = "Configuration" };
        public static readonly PermissionRecord UploadPictures = new PermissionRecord { Name = "上传图片", SystemName = "UploadPictures", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleTasks = new PermissionRecord { Name = "系统任务", SystemName = "ManageScheduleTasks", Category = "Configuration" };

        //public site permissions
        //public static readonly PermissionRecord DisplayPrices = new PermissionRecord { Name = "显示价格", SystemName = "DisplayPrices", Category = "PublicSite" };
        //public static readonly PermissionRecord EnableShoppingCart = new PermissionRecord { Name = "开启购物车", SystemName = "EnableShoppingCart", Category = "PublicSite" };
        //public static readonly PermissionRecord EnableWishlist = new PermissionRecord { Name = "开启收藏夹", SystemName = "EnableWishlist", Category = "PublicSite" };
        //public static readonly PermissionRecord PublicSiteAllowNavigation = new PermissionRecord { Name = "允许访问导航", SystemName = "PublicSiteAllowNavigation", Category = "PublicSite" };


        public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "插件管理", SystemName = "ManagePlugins", Category = "Configuration" };
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                AccessAdminPanel,
                AllowUserImpersonation,
                ManageCatalog,
                ManageChannel,
                ManageUsers,
                ManageUserRoles,
                ManageSettings,
                ManageSites,
                ManageAcl,
                ManageLanguages,
                ManageCountries,
                ManageArticles,
                ManageClients,
                ManageTopics,
                ManageLinks,
                ManageMessageTemplates,
                ManagePlugins,
                ManageWidgets,
                ManageCurrencies,
                ManageQuantityUnits,
                ManageDeliveryTimes,    //codehint: sm-add
                ManageContentSlider,    //codehint: sm-add
                ManageThemes,    //codehint: sm-add
                ManagePolls,
                ManageModelTemplates,
                ManageMeasures ,
                ManageActivityLog , 
                ManageEmailAccounts, 
                ManageSystemLog ,
                ManageMessageQueue,
                ManageMaintenance ,
                UploadPictures ,
                ManageScheduleTasks ,
                ManageExternalAuthenticationMethods,
                ManageRegionalContents,
                ManageFeedbacks,
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                 new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.SuperAdministrators,
                    PermissionRecords = new[] 
                    {
                        AccessAdminPanel,
                        AllowUserImpersonation,
                        ManageCatalog,
                        ManageChannel,
                        ManageUsers,
                        ManageUserRoles,
                        ManageSettings,
                        ManageSites,
                        ManageAcl,
                        ManageLanguages,
                        ManageCountries,
                        ManageArticles,
                        ManageClients,
                        ManageTopics,
                        ManageLinks,

                        ManageMessageTemplates,
                        ManagePlugins,
                        ManageWidgets,
                        ManageCurrencies,
                        ManageQuantityUnits,
                        ManageDeliveryTimes,    //codehint: sm-add
                        ManageContentSlider,    //codehint: sm-add
                        ManageThemes,    //codehint: sm-add
                        ManagePolls,
                        ManageModelTemplates,
                        ManageMeasures ,
                        ManageActivityLog , 
                        ManageEmailAccounts, 
                        ManageSystemLog ,
                        ManageMessageQueue,
                        ManageMaintenance ,
                        UploadPictures ,
                        ManageScheduleTasks ,
                        ManageExternalAuthenticationMethods,
                        ManageRegionalContents,
                         ManageFeedbacks,
                    }
                },
                new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.Administrators,
                    PermissionRecords = new[] 
                    {
                        AccessAdminPanel,
                        AllowUserImpersonation,
                        ManageCatalog,
                        ManageChannel,
                        ManageUsers,
                        ManageUserRoles,
                        ManageSettings,
                        ManageSites,
                        ManageAcl,
                        ManageLanguages,
                        ManageCountries,
                        ManageArticles,
                        ManageClients,
                        ManageTopics,
                        ManageLinks,
                   
                        ManageMessageTemplates,
                        ManagePlugins,
                        ManageWidgets,
                        ManageCurrencies,
                        ManageQuantityUnits,
                        ManageDeliveryTimes,    //codehint: sm-add
                        ManageContentSlider,    //codehint: sm-add
                        ManageThemes,    //codehint: sm-add
                        ManagePolls,
                        ManageModelTemplates,
                        ManageMeasures ,
                        ManageActivityLog , 
                        ManageEmailAccounts, 
                        ManageSystemLog ,
                        ManageMessageQueue,
                        ManageMaintenance ,
                        UploadPictures ,
                        ManageScheduleTasks ,
                        ManageExternalAuthenticationMethods,
                        ManageRegionalContents,
                        ManageFeedbacks,
                    }
                },
                new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.ForumModerators,
                    //PermissionRecords = new[] 
                    //{
                        
                    //}
                },
                new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.Guests,
                                       //PermissionRecords = new[] 
                    //{
                        
                    //}
                },
                new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.Registered,
                                       //PermissionRecords = new[] 
                    //{
                        
                    //}
                },
            };
        }
    }
}