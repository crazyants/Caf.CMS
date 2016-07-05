using System;
using System.ComponentModel;
namespace CAF.WebSite.Application.WebUI
{
	public abstract class SiteMapBase
	{
		private static float defaultCacheDurationInMinutes = 0f;
		private static bool defaultCompress = true;
		private static bool defaultGenerateSearchEngineMap = true;
		private float cacheDurationInMinutes;
		public static float DefaultCacheDurationInMinutes
		{
			get
			{
				return SiteMapBase.defaultCacheDurationInMinutes;
			}
			set
			{
				
				SiteMapBase.defaultCacheDurationInMinutes = value;
			}
		}
		public static bool DefaultCompress
		{
			get
			{
				return SiteMapBase.defaultCompress;
			}
			set
			{
				SiteMapBase.defaultCompress = value;
			}
		}
		public static bool DefaultGenerateSearchEngineMap
		{
			get
			{
				return SiteMapBase.defaultGenerateSearchEngineMap;
			}
			set
			{
				SiteMapBase.defaultGenerateSearchEngineMap = value;
			}
		}
		public SiteMapNode RootNode
		{
			get;
			set;
		}
		public float CacheDurationInMinutes
		{
			get
			{
				return this.cacheDurationInMinutes;
			}
			set
			{
			
				this.cacheDurationInMinutes = value;
			}
		}
		public bool Compress
		{
			get;
			set;
		}
		public bool GenerateSearchEngineMap
		{
			get;
			set;
		}
		protected SiteMapBase()
		{
			this.CacheDurationInMinutes = SiteMapBase.DefaultCacheDurationInMinutes;
			this.Compress = SiteMapBase.DefaultCompress;
			this.GenerateSearchEngineMap = SiteMapBase.DefaultGenerateSearchEngineMap;
			this.RootNode = new SiteMapNode();
		}
		 
		public virtual void Reset()
		{
			this.CacheDurationInMinutes = SiteMapBase.DefaultCacheDurationInMinutes;
			this.Compress = SiteMapBase.DefaultCompress;
			this.GenerateSearchEngineMap = SiteMapBase.DefaultGenerateSearchEngineMap;
			this.RootNode = new SiteMapNode();
		}
	}
}
