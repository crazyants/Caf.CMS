using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CAF.WebSite.Application.WebUI
{

    public static class HtmlHelperExtensions
    {

        public static ComponentFactory CafSite(this HtmlHelper helper)
        {
            return new ComponentFactory(helper);
        }

    }

}
