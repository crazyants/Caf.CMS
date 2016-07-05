using CAF.Infrastructure.MvcHtml;
using System;

namespace AwesomeMvcDemo.Helpers
{
    public static class AwesomeModHelpers
    {
        public static AjaxRadioList<T> Odropdown<T>(this AwesomeHtmlHelper<T> ahtml, string prop, Action<OdropdownCfg> setCfg = null)
        {
            var res = ahtml.AjaxRadioList(prop).Mod("awem.odropdown");
            var odcfg = new OdropdownCfg();

            if (setCfg != null)
            {
                setCfg(odcfg);
                res.Tag(odcfg.ToTag());
            }

            return res;
        }
    }
}