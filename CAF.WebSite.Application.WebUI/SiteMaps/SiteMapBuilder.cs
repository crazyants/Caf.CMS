using System;
using System.ComponentModel;
namespace CAF.WebSite.Application.WebUI
{
	public class SiteMapBuilder 
	{
		private readonly SiteMapBase siteMap;
		private readonly SiteMapNodeBuilder siteMapNodeBuilder;
		public SiteMapNodeBuilder RootNode
		{
			get
			{
				return this.siteMapNodeBuilder;
			}
		}
		public SiteMapBuilder(SiteMapBase siteMap)
		{
			
			this.siteMap = siteMap;
			this.siteMapNodeBuilder = new SiteMapNodeBuilder(this.siteMap.RootNode);
		}
		public static implicit operator SiteMapBase(SiteMapBuilder builder)
		{
		
			return builder.ToSiteMap();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SiteMapBase ToSiteMap()
		{
			return this.siteMap;
		}
		public virtual SiteMapBuilder CacheDurationInMinutes(float value)
		{
			this.siteMap.CacheDurationInMinutes = value;
			return this;
		}
		public virtual SiteMapBuilder Compress(bool value)
		{
			this.siteMap.Compress = value;
			return this;
		}
		public virtual SiteMapBuilder GenerateSearchEngineMap(bool value)
		{
			this.siteMap.GenerateSearchEngineMap = value;
			return this;
		}
	 
	}
}
