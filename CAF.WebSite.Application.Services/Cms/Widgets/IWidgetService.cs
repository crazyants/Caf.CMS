using CAF.Infrastructure.Core.Plugins;
using System.Collections.Generic;
namespace CAF.WebSite.Application.Services.Cms
{
    /// <summary>
    /// Widget service interface
    /// </summary>
    public partial interface IWidgetService
    {
        /// <summary>
        /// Load active widgets
        /// </summary>
		/// <param name="siteId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
		IEnumerable<Provider<IWidget>> LoadActiveWidgets(int siteId = 0);

        
        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
		/// <param name="siteId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
		IEnumerable<Provider<IWidget>> LoadActiveWidgetsByWidgetZone(string widgetZone, int siteId = 0);

        /// <summary>
        /// Load widget by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget</returns>
		Provider<IWidget> LoadWidgetBySystemName(string systemName);

        /// <summary>
        /// Load all widgets
        /// </summary>
		/// <param name="siteId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
		IEnumerable<Provider<IWidget>> LoadAllWidgets(int siteId = 0);
    }
}
