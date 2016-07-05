
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CAF.WebSite.Application.WebUI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class JsonFilter : ActionFilterAttribute
    {
        public string Param { get; set; }
        public Type JsonDataType { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.ContentType.Contains("application/json"))
            {
                string inputContent;
                using (var sr = new StreamReader(filterContext.HttpContext.Request.InputStream))
                {
                    inputContent = sr.ReadToEnd();
                }
                JsonSerializer serializer = new JsonSerializer();
                StringReader srJson = new StringReader(inputContent);
                var result = serializer.Deserialize(new JsonTextReader(srJson), JsonDataType);
                filterContext.ActionParameters[Param] = result;
            }
        }
    }

}
