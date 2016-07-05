using System;
using System.Web.Mvc;
using System.Linq;
using CAF.Infrastructure.LinqSearchModel.Model;
using System.Collections.Generic;
 


namespace CAF.Mvc.JQuery.Datatables.Core
{
    /// <summary>
    /// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
    /// </summary>
    public class DataTablesEditorModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProvider = bindingContext.ValueProvider;

            DtEditorRequest model = new DtEditorRequest(controllerContext.HttpContext.Request.Form);

            return model;

         
        }

    }
}