using System;
using System.Collections.Generic;
using CAF.Infrastructure.Core.DependencyManagement;
using CAF.Infrastructure.Core.Configuration;

namespace CAF.Infrastructure.Core
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the CrAnt.FrameWork. engine. Edit functionality, modules
    /// and implementations access most CrAnt.FrameWork. functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {

        ContainerManager ContainerManager { get; }

        /// <summary>
        /// Initialize components and plugins in the CAF environment.
        /// </summary>
        /// <param name="config">Config</param>
        void Initialize();

        T Resolve<T>(string name = null) where T : class;

        object Resolve(Type type, string name = null);

        T[] ResolveAll<T>();
    }
}
