using System;
using System.Web;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Application.WebUI.Theming
{
    public class LocalizedDisplayMode : System.Web.WebPages.DefaultDisplayMode
    {
        private readonly bool _enabled;

        public LocalizedDisplayMode(bool enabled)
            : this(System.Web.WebPages.DisplayModeProvider.DefaultDisplayModeId, enabled)
        {
        }

        public LocalizedDisplayMode(string suffix, bool enables)
            : base(suffix)
        {
            _enabled = enables;
        }

        public override System.Web.WebPages.DisplayInfo GetDisplayInfo(HttpContextBase httpContext, string virtualPath, Func<string, bool> virtualPathExists)
        {
            if (!_enabled)
            {
                // default behaviour, because localized views are not enabled
                return base.GetDisplayInfo(httpContext, virtualPath, virtualPathExists);
            }

            var lang = GetCurrentLanguageSeoCode();

            var result =
                GetDisplayInfoInternal(httpContext, virtualPath, lang, virtualPathExists) ??
                GetDisplayInfoInternal(httpContext, virtualPath, null, virtualPathExists);

            return result;
        }

        private System.Web.WebPages.DisplayInfo GetDisplayInfoInternal(HttpContextBase httpContext, string virtualPath, string lang, Func<string, bool> virtualPathExists)
        {
            string path = this.TransformPath(virtualPath, "{0}{1}".FormatInvariant(base.DisplayModeId, lang.IsEmpty() ? "" : "." + lang));
            if (path != null && virtualPathExists(path))
            {
                return new LocalizedDisplayInfo(path, this, lang);
            }

            return null;
        }

        public override string DisplayModeId
        {
            get
            {
                if (!_enabled)
                {
                    return base.DisplayModeId;
                }

                var lang = GetCurrentLanguageSeoCode();
                return "{0}{1}".FormatInvariant(base.DisplayModeId, lang.IsEmpty() ? "" : "." + lang);
            }
        }

        protected string GetCurrentLanguageSeoCode()
        {
            string result = null;

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            if (workContext != null && workContext.WorkingLanguage != null)
            {
                result = workContext.WorkingLanguage.UniqueSeoCode;
            }

            return result.NullEmpty() ?? "en";
        }
    }

    public class LocalizedDisplayInfo : System.Web.WebPages.DisplayInfo
    {
        public LocalizedDisplayInfo(string filePath, System.Web.WebPages.IDisplayMode displayMode, string lang)
            : base(filePath, displayMode)
        {
            this.Lang = lang;
        }

        public string Lang { get; set; }

        public string DisplayModeId
        {
            get
            {
                return "{0}{1}".FormatInvariant(DisplayMode.DisplayModeId, Lang.IsEmpty() ? "" : "." + Lang);
            }
        }
    }

}
