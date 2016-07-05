using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Localization;
using System.Reflection;
using System.Linq;
using CAF.WebSite.Application.WebUI;
namespace CAF.Mvc.JQuery.Datatables.Core
{
    internal static class GridTypeInfoHelper
    {

        public static string ToDisplayName(PropertyInfo pi)
        {
            //获取语言包显示名称
            var langDisplayName = (pi.GetCustomAttributes()).OfType<LangResourceDisplayName>().FirstOrDefault();
            if (langDisplayName != null)
            {
                string displayName = langDisplayName.DisplayName;
                if (!string.IsNullOrEmpty(displayName))
                    return displayName;
            }

            return "";
        }

    }
}