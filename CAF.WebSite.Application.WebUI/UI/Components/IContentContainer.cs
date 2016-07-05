using System;
using System.Collections.Generic;
using System.Web.WebPages;

namespace CAF.WebSite.Application.WebUI
{

    public interface IContentContainer
    {

        IDictionary<string, object> ContentHtmlAttributes
        {
            get;
        }

        HelperResult Content
        {
            get;
            set;
        }

    }

}
