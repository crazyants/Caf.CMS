using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Authentication.External;
using CAF.WebSite.Mvc.Models.Users;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace CAF.WebSite.Mvc.Controllers
{
    /// <summary>
    /// 第三方登陆验证
    /// </summary>
    public partial class ExternalAuthenticationController : PublicControllerBase
    {
		#region Fields

        private readonly IOpenAuthenticationService _openAuthenticationService;
		private readonly ISiteContext _siteContext;

        #endregion

		#region Constructors

        public ExternalAuthenticationController(IOpenAuthenticationService openAuthenticationService,
			ISiteContext siteContext)
        {
            this._openAuthenticationService = openAuthenticationService;
			this._siteContext = siteContext;
        }

        #endregion

        #region Methods

        public RedirectResult RemoveParameterAssociation(string returnUrl)
        {
            ExternalAuthorizerHelper.RemoveParameters();
            return Redirect(returnUrl);
        }

        [ChildActionOnly]
        public ActionResult ExternalMethods()
        {
            //model
            var model = new List<ExternalAuthenticationMethodModel>();

			foreach (var eam in _openAuthenticationService.LoadActiveExternalAuthenticationMethods(_siteContext.CurrentSite.Id))
            {
                var eamModel = new ExternalAuthenticationMethodModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                eam.Value.GetPublicInfoRoute(out actionName, out controllerName, out routeValues);
                eamModel.ActionName = actionName;
                eamModel.ControllerName = controllerName;
                eamModel.RouteValues = routeValues;

                model.Add(eamModel);
            }

            return PartialView(model);
        }

        #endregion
    }
}
