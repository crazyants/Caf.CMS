using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Reflection;
using Autofac.Core;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.CustomBanner.Services;
using CAF.Infrastructure.Core.Data;
using CAF.WebSite.CustomBanner.Data;
using CAF.Infrastructure.Data;
using CAF.WebSite.CustomBanner.Domain;
using Autofac;

namespace CAF.WebSite.CustomBanner
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
            builder.RegisterType<CustomBannerService>().As<ICustomBannerService>().InstancePerRequest();

            //data layer
            //register named context
            builder.Register<IDbContext>(c => new CustomBannerObjectContext(DataSettings.Current.DataConnectionString))
                .Named<IDbContext>(CustomBannerObjectContext.ALIASKEY)
                .InstancePerRequest();

            builder.Register<CustomBannerObjectContext>(c => new CustomBannerObjectContext(DataSettings.Current.DataConnectionString))
                .InstancePerRequest();

            //override required repository with our custom context
            builder.RegisterType<EfRepository<CustomBannerRecord>>()
                .As<IRepository<CustomBannerRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CustomBannerObjectContext.ALIASKEY))
                .InstancePerRequest();
        }
    }
}
