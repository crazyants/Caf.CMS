﻿using System.IO;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Text;
using System;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Utilities;
namespace CAF.WebSite.Application.WebUI.Controllers
{
    public static class ContollerExtensions
    {
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller)
        {
            return RenderPartialViewToString(controller, null, null, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller, string viewName)
        {
            return RenderPartialViewToString(controller, viewName, null, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller, object model)
        {
            return RenderPartialViewToString(controller, null, model, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller, string viewName, object model)
        {
            return RenderPartialViewToString(controller, viewName, model, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <param name="additionalViewData">Additional ViewData</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this ControllerBase controller, string viewName, object model, object additionalViewData)
        {
            if (viewName.IsEmpty())
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            if (additionalViewData != null)
            {
                controller.ViewData.AddRange(CollectionHelper.ObjectToDictionary(additionalViewData));
            }

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName.EmptyNull());

                ThrowIfViewNotFound(viewResult, viewName);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller)
        {
            return RenderViewToString(controller, null, null, null);
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller, object model)
        {
            return RenderViewToString(controller, null, null, model);
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller, string viewName)
        {
            return RenderViewToString(controller, viewName, null, null);
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller, string viewName, string masterName)
        {
            return RenderViewToString(controller, viewName, masterName, null);
        }

        /// <summary>
        /// Render view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="masterName">Master name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderViewToString(this ControllerBase controller, string viewName, string masterName, object model)
        {
            if (viewName.IsEmpty())
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(controller.ControllerContext, viewName.EmptyNull(), masterName.EmptyNull());

                ThrowIfViewNotFound(viewResult, viewName);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        private static void ThrowIfViewNotFound(ViewEngineResult viewResult, string viewName)
        {
            // if view not found, throw an exception with searched locations
            if (viewResult.View == null)
            {
                var locations = new StringBuilder();
                locations.AppendLine();

                foreach (string location in viewResult.SearchedLocations)
                {
                    locations.AppendLine(location);
                }

                throw new InvalidOperationException(string.Format("The view '{0}' or its master was not found, searched locations: {1}", viewName, locations));
            }
        }

        /// <summary>
        /// Get active site scope (for multi-site configuration mode)
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="siteService">Site service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Site ID; 0 if we are in a shared mode</returns>
        public static int GetActiveSiteScopeConfiguration(this Controller controller, ISiteService siteService, IWorkContext workContext)
        {
            //ensure that we have 2 (or more) sites
            if (siteService.GetAllSites().Count < 2)
                return 0;

            var siteId = workContext.CurrentUser.GetAttribute<int>(SystemUserAttributeNames.AdminAreaSiteScopeConfiguration);
            var site = siteService.GetSiteById(siteId);
            return site != null ? site.Id : 0;
        }
    }
}
