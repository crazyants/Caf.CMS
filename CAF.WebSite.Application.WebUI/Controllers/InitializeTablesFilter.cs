using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Utilities;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Tasks;
using CAF.WebSite.Application.Services.Tasks;
using System.Collections;
using System.Reflection;
using CAF.WebSite.Application.WebUI.UI.Grid;
using CAF.WebSite.Application.Services.Tables;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class InitializeTablesFilter : IAuthorizationFilter
    {
        private readonly static object s_lock = new object();
        private static bool s_initializing = false;

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            lock (s_lock)
            {
                if (!s_initializing)
                {
                    s_initializing = true;

                    var logger = EngineContext.Current.Resolve<ILogger>();

                    try
                    {
                        //var tableService = EngineContext.Current.Resolve<ITableConfigService>();
                        //var siteService = EngineContext.Current.Resolve<ISiteService>();
                        //var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
                        //var tableExecutor = EngineContext.Current.Resolve<ITableExecutor>();

                        //var tables = tableService.GetAllTableConfigs(true);
                        //foreach (var table in tables)
                        //{
                        //    tableExecutor.Execute(table);
                        //}
                        var gridPublisher = EngineContext.Current.Resolve<IGridPublisher>();
                        gridPublisher.RegisterGrid();

                        //var gridRegistrationTypes = FilterTypesInAssemblies(IsGridRegistrationType);

                        //foreach (Type gridRegistrationType in gridRegistrationTypes)
                        //{

                        //    IGridRegistration registration = (IGridRegistration)gridRegistrationType;
                        //    registration.RegisterGrids();
                        //}
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error while initializing Task Scheduler", ex);
                    }
                    finally
                    {
                        GlobalFilters.Filters.Remove(this);
                    }
                }
            }
        }

        private  bool IsGridRegistrationType(Type type)
        {
            return
                typeof(IGridRegistration).IsAssignableFrom(type) &&
                type.GetConstructor(Type.EmptyTypes) != null;
        }

        private  IEnumerable<Type> FilterTypesInAssemblies(Predicate<Type> predicate)
        {
            // Go through all assemblies referenced by the application and search for types matching a predicate
            IEnumerable<Type> typesSoFar = Type.EmptyTypes;

            ICollection assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] typesInAsm;
                try
                {
                    typesInAsm = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    typesInAsm = ex.Types;
                }
                typesSoFar = typesSoFar.Concat(typesInAsm);
            }
            return typesSoFar.Where(type => TypeIsPublicClass(type) && predicate(type));
        }

        private  bool TypeIsPublicClass(Type type)
        {
            return (type != null && type.IsPublic && type.IsClass && !type.IsAbstract);
        }
    }
}
