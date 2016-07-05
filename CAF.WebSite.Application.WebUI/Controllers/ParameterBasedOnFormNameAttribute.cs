using System.Web.Mvc;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    /// <summary>
    /// If form name exists, then specified "actionParameterName" will be set to "true"
    /// </summary>
    public class ParameterBasedOnFormNameAttribute : FilterAttribute, IActionFilter
    {
        private readonly string _name1;
        private readonly string _actionParameterName1;
        private readonly string _name2;
        private readonly string _actionParameterName2;

        public ParameterBasedOnFormNameAttribute(string name, string actionParameterName)
        {
            this._name1 = name;
            this._actionParameterName1 = actionParameterName;
        }
        public ParameterBasedOnFormNameAttribute(string name1, string actionParameterName1, string name2, string actionParameterName2)
        {
            this._name1 = name1;
            this._actionParameterName1 = actionParameterName1;
            this._name2 = name2;
            this._actionParameterName2 = actionParameterName2;
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_name1.IsEmpty())
            {
                var formValue = filterContext.RequestContext.HttpContext.Request.Form[_name1];
                filterContext.ActionParameters[_actionParameterName1] = !string.IsNullOrEmpty(formValue);
            }
            if (!_name2.IsEmpty())
            {
                var formValue = filterContext.RequestContext.HttpContext.Request.Form[_name2];
                filterContext.ActionParameters[_actionParameterName2] = !string.IsNullOrEmpty(formValue);
            }
        }
    }
}
