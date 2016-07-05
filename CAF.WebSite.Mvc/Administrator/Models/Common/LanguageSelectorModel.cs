using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Models.Localization;
using System.Collections.Generic;

namespace CAF.WebSite.Mvc.Admin.Models.Common
{
    public class LanguageSelectorModel : ModelBase
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }
    }
}