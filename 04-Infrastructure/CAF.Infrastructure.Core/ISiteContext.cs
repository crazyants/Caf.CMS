
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CAF.Infrastructure.Core
{
	/// <summary>
	/// Site context
	/// </summary>
    public interface ISiteContext
	{
		/// <summary>
		/// Sets a store override to be used for the current request
		/// </summary>
		/// <param name="siteId">The store override or <c>null</c> to remove the override</param>
		void SetRequestSite(int? siteId);

		/// <summary>
		/// Sets a store override to be used for the current user's session (e.g. for preview mode)
		/// </summary>
		/// <param name="siteId">The store override or <c>null</c> to remove the override</param>
		void SetPreviewSite(int? siteId);

		/// <summary>
		/// Gets the store override for the current request
		/// </summary>
		/// <returns>The store override or <c>null</c></returns>
		int? GetRequestSite();

		/// <summary>
		/// Gets the store override for the current session
		/// </summary>
		/// <returns>The store override or <c>null</c></returns>
		int? GetPreviewSite();
		
		/// <summary>
		/// Gets or sets the current store
		/// </summary>
		Site CurrentSite { get; }

		/// <summary>
		/// IsSingleSiteMode ? 0 : CurrentSite.Id
		/// </summary>
		/// <remarks>codehint: sm-add</remarks>
		int CurrentSiteIdIfMultiSiteMode { get; }
	}
}
