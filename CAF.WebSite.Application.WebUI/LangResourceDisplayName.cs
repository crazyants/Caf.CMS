
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core;
using System;
using System.Runtime.CompilerServices;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI.Mvc;


namespace CAF.WebSite.Application.WebUI
{
    public class LangResourceDisplayName : System.ComponentModel.DisplayNameAttribute, IModelAttribute
    {
        private readonly string _callerPropertyName;

        public LangResourceDisplayName(string resourceKey, [CallerMemberName] string propertyName = null)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
            _callerPropertyName = propertyName;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                string value = null;
                var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                value = EngineContext.Current.Resolve<ILocalizationService>()
                        .GetResource(ResourceKey, langId, true, "" /* ResourceKey */, true);

                if (value.IsEmpty() && _callerPropertyName.HasValue())
                {
                    value = _callerPropertyName.SplitPascalCase();
                }

                return value;
            }
        }

        public string Name
        {
            get { return "LangResourceDisplayName"; }
        }
    }
}
