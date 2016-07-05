using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
 

namespace CAF.WebSite.Application.WebUI.UI.Grid
{
    public class GridPublisher : IGridPublisher
    {
        private readonly ITypeFinder _typeFinder;

        public GridPublisher(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public void RegisterGrid()
        {
            var routeProviderTypes = _typeFinder.FindClassesOfType<IGridRegistration>();

            foreach (var providerType in routeProviderTypes)
            {
                var registration = Activator.CreateInstance(providerType) as IGridRegistration;
                registration.RegisterGrids();
            }
          
        }
    }
}
