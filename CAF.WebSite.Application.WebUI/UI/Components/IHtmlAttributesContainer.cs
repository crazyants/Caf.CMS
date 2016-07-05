using System;
using System.Collections.Generic;

namespace CAF.WebSite.Application.WebUI
{

    public interface IHtmlAttributesContainer
    {

        IDictionary<string, object> HtmlAttributes
        {
            get;
        }

    }

}
