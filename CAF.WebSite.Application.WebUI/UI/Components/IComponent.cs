using System;

namespace CAF.WebSite.Application.WebUI
{

    public interface IComponent : IHtmlAttributesContainer
    {

        string Id
        {
            get;
        }

        string Name
        {
            get;
        }

        bool NameIsRequired
        {
            get;
        }

    }

}
