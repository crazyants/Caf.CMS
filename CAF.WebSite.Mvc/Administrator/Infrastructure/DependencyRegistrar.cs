using Autofac;
using Autofac.Integration.Mvc;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.DependencyManagement;

 

namespace CAF.WebSite.Mvc.Admin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
        {
			if (DataSettings.DatabaseIsInstalled())
			{
				 
			}
        }

        public int Order
        {
            get { return 100; }
        }
    }
}
