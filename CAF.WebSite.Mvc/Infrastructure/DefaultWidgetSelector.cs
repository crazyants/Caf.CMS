using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Cms;
using CAF.WebSite.Application.Services.Topics;
using CAF.WebSite.Application.WebUI.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using CAF.Infrastructure.Core.Collections;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Models.Topics;


namespace CAF.WebSite.Mvc.Infrastructure
{
    
    public partial class DefaultWidgetSelector : IWidgetSelector
    {

        #region Fields

        private readonly IWidgetService _widgetService;
        private readonly ITopicService _topicService;
        private readonly ISiteContext _siteContext;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
		private readonly IWidgetProvider _widgetProvider;
		private readonly ICommonServices _services;

        #endregion

        public DefaultWidgetSelector(
            IWidgetService widgetService, 
            ITopicService topicService, 
            ISiteContext siteContext, 
            ICacheManager cacheManager, 
            IWorkContext workContext, 
            IDbContext dbContext,
			IWidgetProvider widgetProvider,
			ICommonServices services)
        {
            this._widgetService = widgetService;
            this._topicService = topicService;
            this._siteContext = siteContext;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
            this._dbContext = dbContext;
			this._widgetProvider = widgetProvider;
			this._services = services;
        }

        public virtual IEnumerable<WidgetRouteInfo> GetWidgets(string widgetZone, object model)
        {
			string actionName;
            string controllerName;
            RouteValueDictionary routeValues;
			var siteId = _siteContext.CurrentSite.Id;

            #region Plugin Widgets

			var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone, siteId);
            foreach (var widget in widgets)
            {
                widget.Value.GetDisplayWidgetRoute(widgetZone, model, siteId, out actionName, out controllerName, out routeValues);

				if (actionName.HasValue() && controllerName.HasValue())
				{
					yield return new WidgetRouteInfo
					{
						ActionName = actionName,
						ControllerName = controllerName,
						RouteValues = routeValues
					};
				}
            }

            #endregion


            #region Topic Widgets

            // add special "topic widgets" to the list
			var allTopicsCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_WIDGET_ALL_MODEL_KEY, siteId, _workContext.WorkingLanguage.Id);
            // get topic widgets from STATIC cache
			var topicWidgets = _services.Cache.Get(allTopicsCacheKey, () =>
            {
				using (var scope = new DbContextScope(forceNoTracking: true))
				{
					var allTopicWidgets = _topicService.GetAllTopics(siteId).Where(x => x.RenderAsWidget).ToList();
					var stubs = allTopicWidgets
						.Select(t => new TopicWidgetStub
						{
							Id = t.Id,
							Bordered = t.WidgetBordered,
							ShowTitle = t.WidgetShowTitle,
							SystemName = t.SystemName.SanitizeHtmlId(),
							Title = t.GetLocalized(x => t.Title),
							Body = t.GetLocalized(x => t.Body),
							WidgetZones = t.GetWidgetZones().ToArray(),
							Priority = t.Priority
						})
						.OrderBy(t => t.Priority)
						.ToList();
					return stubs;
				}
            });

            var byZoneTopicsCacheKey = "CafSite.TopicWidgets.ZoneMapped";
            // save widgets to zones map in request cache
			var topicsByZone = _cacheManager.Get(byZoneTopicsCacheKey, () =>
            {
				var map = new Multimap<string, WidgetRouteInfo>();

				foreach (var widget in topicWidgets)
				{
					var zones = widget.WidgetZones;
					if (zones != null && zones.Any())
					{
						foreach (var zone in zones.Select(x => x.ToLower()))
						{
							var routeInfo = new WidgetRouteInfo
							{
								ControllerName = "Topic",
								ActionName = "TopicWidget",
								RouteValues = new RouteValueDictionary()
								{
									{"Namespaces", "CafSite.Web.Controllers"},
									{"area", null},
									{"widgetZone", zone},
									{"model", new TopicWidgetModel 
									{ 
										Id = widget.Id,
										SystemName = widget.SystemName,
										ShowTitle = widget.ShowTitle,
										IsBordered = widget.Bordered,
										Title = widget.Title,
										Html = widget.Body
									} }
								}
							};
							map.Add(zone, routeInfo);
						}
					}
				}

				return map;

				#region Obsolete
				//var result = from t in topicWidgets 
				//			 where t.WidgetZones.Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)
				//			 orderby t.Priority
				//			 select new WidgetRouteInfo
				//			 {
				//				 ControllerName = "Topic",
				//				 ActionName = "TopicWidget",
				//				 RouteValues = new RouteValueDictionary()
				//				 {
				//					{"Namespaces", "CafSite.Web.Controllers"},
				//					{"area", null},
				//					{"widgetZone", widgetZone},
				//					{"model", new TopicWidgetModel 
				//					{ 
				//						Id = t.Id,
				//						SystemName = t.SystemName,
				//						ShowTitle = t.ShowTitle,
				//						IsBordered = t.Bordered,
				//						Title = t.Title,
				//						Html = t.Body
				//					} }
				//				 }
				//			 };

				//return result.ToList(); 
				#endregion
			});

			if (topicsByZone.ContainsKey(widgetZone.ToLower()))
			{
				var zoneWidgets = topicsByZone[widgetZone.ToLower()];
				foreach (var topicWidget in zoneWidgets)
				{
					yield return topicWidget;
				}
			}

            #endregion


			#region Request scoped widgets (provided by IWidgetProvider)

			var requestScopedWidgets = _widgetProvider.GetWidgets(widgetZone);
			if (requestScopedWidgets != null)
			{
				foreach (var widget in requestScopedWidgets)
				{
					yield return widget;
				}
			}

			#endregion
        }

		class TopicWidgetStub
		{
			public int Id { get; set; }
			public string[] WidgetZones { get; set; }
			public string SystemName { get; set; }
			public bool ShowTitle { get; set; }
			public bool Bordered { get; set; }
			public string Title { get; set; }
			public string Body { get; set; }
			public int Priority { get; set; }
		}

    }

}