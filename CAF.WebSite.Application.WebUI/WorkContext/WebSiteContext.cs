using System;
using System.Linq;
using System.Web;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.WebSite.Application.Services.Sites;



namespace CAF.WebSite.Application.WebUI
{
	/// <summary>
	/// Site context for web application
	/// </summary>
    public partial class WebSiteContext : ISiteContext
	{
		internal const string OverriddenSiteIdKey = "OverriddenSiteId";

        private readonly ISiteService _SiteService;
		private readonly IWebHelper _webHelper;
		private readonly HttpContextBase _httpContext;

        private Site _currentSite;

        public WebSiteContext(ISiteService SiteService, IWebHelper webHelper, HttpContextBase httpContext)
		{
            this._SiteService = SiteService;
			this._webHelper = webHelper;
			this._httpContext = httpContext;
		}

		public void SetRequestSite(int? siteId)
		{
			try
			{
				var dataTokens = _httpContext.Request.RequestContext.RouteData.DataTokens;
				if (siteId.GetValueOrDefault() > 0)
				{
					dataTokens[OverriddenSiteIdKey] = siteId.Value;
				}
				else if (dataTokens.ContainsKey(OverriddenSiteIdKey))
				{
					dataTokens.Remove(OverriddenSiteIdKey);
				}

				_currentSite = null;
			}
			catch { }
		}

		public int? GetRequestSite()
		{
			try
			{
				var value = _httpContext.Request.RequestContext.RouteData.DataTokens[OverriddenSiteIdKey];
				if (value != null)
				{
					return (int)value;
				}

				return null;
			}
			catch
			{
				return null;
			}
		}

		public void SetPreviewSite(int? siteId)
		{
			try
			{
                _httpContext.SetPreviewModeValue(OverriddenSiteIdKey, siteId.HasValue ? siteId.Value.ToString() : null);
				_currentSite = null;
			}
			catch { }
		}

		public int? GetPreviewSite()
		{
			try
			{
				var cookie = _httpContext.GetPreviewModeCookie(false);
				if (cookie != null)
				{
					var value = cookie.Values[OverriddenSiteIdKey];
					if (value.HasValue())
					{
						return value.ToInt();
					}
				}

				return null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets or sets the current store
		/// </summary>
		public Site CurrentSite
		{
			get
			{	
				if (_currentSite == null)
				{
					int? storeOverride = GetRequestSite() ?? GetPreviewSite();
					if (storeOverride.HasValue)
					{
						// the store to be used can be overwritten on request basis (e.g. for theme preview, editing etc.)
                        _currentSite = _SiteService.GetSiteById(storeOverride.Value);
					}
					else
					{
						// ty to determine the current store by HTTP_HOST
						var host = _webHelper.ServerVariables("HTTP_HOST");
                        var allSites = _SiteService.GetAllSites();
						var store = allSites.FirstOrDefault(s => s.ContainsHostValue(host));

						if (store == null)
						{
							//load the first found store
							store = allSites.FirstOrDefault();
						}

						if (store == null)
						{
							throw new Exception("No store could be loaded");
						}

						_currentSite = store;
					}
				}

				return _currentSite;
			}
		}

		/// <summary>
		/// IsSingleSiteMode ? 0 : CurrentSite.Id
		/// </summary>
		public int CurrentSiteIdIfMultiSiteMode
		{
			get
			{
                return  CurrentSite.Id;
			}
		}

	}
}
