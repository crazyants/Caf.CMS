using System.Web.Mvc;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    /// <summary>
    /// If form name exists, then specified "actionParameterName" will be set to "true"
    /// </summary>
    public class ParameterBasedOnQueryNameAttribute : FilterAttribute, IActionFilter
    {
        private readonly string _name;
        private readonly string _actionParameterName;

        public ParameterBasedOnQueryNameAttribute(string name, string actionParameterName)
        {
            this._name = name;
            this._actionParameterName = actionParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var formValue = filterContext.RequestContext.HttpContext.Request.QueryString[_name];
            filterContext.ActionParameters[_actionParameterName] = !string.IsNullOrEmpty(formValue);
        }
    }
}
