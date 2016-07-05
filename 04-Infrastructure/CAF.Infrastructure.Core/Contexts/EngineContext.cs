using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Utilities;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace CAF.Infrastructure.Core
{
    /// <summary>
    /// Provides access to the singleton instance of the CrAnt.FrameWork. engine.
    /// </summary>
    public class EngineContext
    {
        #region Initialization Methods
        /// <summary>Initializes a static instance of the CAF factory.</summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                Singleton<IEngine>.Instance = CreateEngineInstance();
                Singleton<IEngine>.Instance.Initialize();
            }
            return Singleton<IEngine>.Instance;
        }

        /// <summary>Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.</summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        /// <summary>
        /// Creates a factory instance and adds a http application injecting facility.
        /// </summary>
        /// <returns>A new factory</returns>
        public static IEngine CreateEngineInstance()
        {
            var engineTypeSetting = CommonHelper.GetAppSetting<string>("caf:EngineType");
            if (engineTypeSetting.HasValue())
            {
                var engineType = Type.GetType(engineTypeSetting);
                if (engineType == null)
                    throw new ConfigurationErrorsException("The type '" + engineType + "' could not be found. Please check the configuration at /configuration/appSettings/add[@key=caf:EngineType] or check for missing assemblies.");
                if (!typeof(IEngine).IsAssignableFrom(engineType))
                    throw new ConfigurationErrorsException("The type '" + engineType + "' doesn't implement 'CAF.Core.Infrastructure.IEngine' and cannot be configured in /configuration/appSettings/add[@key=caf:EngineType] for that purpose.");
                return Activator.CreateInstance(engineType) as IEngine;
            }

            return new WorkEngine();
        }

        #endregion

        /// <summary>Gets the singleton CAF engine used to access CAF services.</summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }
    }
}
