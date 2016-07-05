using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using CAF.WebSite.DevTools.Filters;
using CAF.WebSite.DevTools.Services;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI.Controllers;


namespace CAF.WebSite.DevTools
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
			builder.RegisterType<ProfilerService>().As<IProfilerService>().InstancePerRequest();

			if (isActiveModule)
			{
				// intercept ALL public store controller actions
				builder.RegisterType<ProfilerFilter>().AsActionFilterFor<BaseController>();

				//// intercept CatalogController's Product action
				//builder.RegisterType<SampleResultFilter>().AsResultFilterFor<CatalogController>(x => x.Product(default(int), default(string))).InstancePerRequest();
				//builder.RegisterType<SampleActionFilter>().AsActionFilterFor<PublicControllerBase>().InstancePerRequest();
				//// intercept CheckoutController's Index action (to hijack the checkout or payment workflow)
				//builder.RegisterType<SampleCheckoutFilter>().AsActionFilterFor<CheckoutController>(x => x.Index()).InstancePerRequest();
			}
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
