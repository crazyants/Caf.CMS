using System.Collections.Generic;

namespace CAF.WebSite.Application.WebUI.Localization
{
    public interface ILocalizedModel
    {

    }
    public interface ILocalizedModel<TLocalizedModel> : ILocalizedModel
    {
        #region Data Members (1)

        IList<TLocalizedModel> Locales { get; set; }

        #endregion Data Members
    }
}
