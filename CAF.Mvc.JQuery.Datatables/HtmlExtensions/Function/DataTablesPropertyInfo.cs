
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Reflection;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    class DataTablesPropertyInfo
    {
        public DataTablesPropertyInfo(System.Reflection.PropertyInfo propertyInfo, IModelAttribute[] attributeses)
        {
            PropertyInfo = propertyInfo;
            Attributes = attributeses;
        }

        public System.Reflection.PropertyInfo PropertyInfo { get; private set; }
        public IModelAttribute[] Attributes { get; private set; }
        public Type Type {
            get { return this.PropertyInfo.PropertyType; }
        }
    }
}