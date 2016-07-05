using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI.UI;
using CAF.WebSite.Mvc.Controllers;
using CAF.WebSite.Mvc.Infrastructure.Installation;
using CAF.Infrastructure.Data.Setup;


namespace CAF.WebSite.Mvc.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
            //we cache presentation models between requests
            builder.RegisterType<ArticleController>().WithStaticCache();
            builder.RegisterType<ArticleCatalogController>().WithStaticCache();
            builder.RegisterType<CountryController>().WithStaticCache();
            builder.RegisterType<CommonController>().WithStaticCache();
            builder.RegisterType<TopicController>().WithStaticCache();

            builder.RegisterType<ArticleCatalogHelper>().InstancePerRequest();

			//we cache presentation models between requests
			builder.RegisterType<DefaultWidgetSelector>().As<IWidgetSelector>().InstancePerRequest();
            builder.RegisterType<ArticleCatalogHelper>().InstancePerRequest();

            // installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerRequest();

            // register app languages for installation
            //builder.RegisterType<EnUSSeedData>()
            //    .As<InvariantSeedData>()
            //    .WithMetadata<InstallationAppLanguageMetadata>(m =>
            //    {
            //        m.For(em => em.Culture, "en-US");
            //        m.For(em => em.Name, "English");
            //        m.For(em => em.UniqueSeoCode, "en");
            //        m.For(em => em.FlagImageFileName, "us.png");
            //    })
            //    .InstancePerRequest();
            builder.RegisterType<ZhCNSeedData>()
                .As<InvariantSeedData>()
                .WithMetadata<InstallationAppLanguageMetadata>(m =>
                {
                    m.For(em => em.Culture, "zh-CN");
                    m.For(em => em.Name, "ÖÐÎÄ");
                    m.For(em => em.UniqueSeoCode, "zh");
                    m.For(em => em.FlagImageFileName, "cn.png");
                })
                .InstancePerRequest();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
