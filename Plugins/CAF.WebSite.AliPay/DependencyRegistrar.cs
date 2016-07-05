using Autofac;
using Autofac.Integration.Mvc;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.DependencyManagement;

namespace CAF.WebSite.AliPay
{
	public class DependencyRegistrar : IDependencyRegistrar
	{
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
		{
			//builder.RegisterType<AmazonPayService>().As<IAmazonPayService>().InstancePerRequest();
			//builder.RegisterType<AmazonPayApi>().As<IAmazonPayApi>().InstancePerRequest();

            //if (isActiveModule)
            //{
            //    builder.RegisterType<AmazonPayCheckoutFilter>().AsActionFilterFor<CheckoutController>().InstancePerRequest();
            //}
		}

		public int Order
		{
			get { return 1; }
		}
	}
}
