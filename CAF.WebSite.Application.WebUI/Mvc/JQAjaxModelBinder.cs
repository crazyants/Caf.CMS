using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Application.WebUI.Mvc
{
    public class JQAjaxModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType.IsEnumerable())
            {
                var key = bindingContext.ModelName + "[]";
                var valueResult = bindingContext.ValueProvider.GetValue(key);
                if (valueResult != null && !string.IsNullOrEmpty(valueResult.AttemptedValue))
                {
                    bindingContext.ModelName = key;
                }
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
