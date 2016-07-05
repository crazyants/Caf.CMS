using Autofac;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.WebSite.QQAuth.Core;


namespace CAF.WebSite.QQAuth
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
             builder.RegisterType<QQProviderAuthorizer>().As<IOAuthProviderQQAuthorizer>().InstancePerRequest();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
