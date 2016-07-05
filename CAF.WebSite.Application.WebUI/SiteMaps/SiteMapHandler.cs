using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using Telerik.Web.Mvc.Extensions;
using Telerik.Web.Mvc.Infrastructure;
namespace CAF.WebSite.Application.WebUI
{
	public class SiteMapHandler : HttpHandlerBase
	{
		private const string SiteMapNameSpace = "http://www.sitemaps.org/schemas/sitemap/0.9";
		private readonly SiteMapDictionary siteMaps;
		private readonly IHttpResponseCompressor httpResponseCompressor;
		private readonly IHttpResponseCacher httpResponseCacher;
		private readonly IUrlGenerator urlGenerator;
		private readonly IDictionary<string, string> duplicateChecks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private static string defaultPath = "~/sitemap.axd";
		public static string DefaultPath
		{
			get
			{
				return SiteMapHandler.defaultPath;
			}
			set
			{
				Guard.IsNotNullOrEmpty(value, "value");
				SiteMapHandler.defaultPath = value;
			}
		}
		public SiteMapHandler(SiteMapDictionary siteMaps, IHttpResponseCompressor httpResponseCompressor, IHttpResponseCacher httpResponseCacher, IUrlGenerator urlGenerator)
		{
			Guard.IsNotNull(siteMaps, "siteMaps");
			Guard.IsNotNull(httpResponseCompressor, "httpResponseCompressor");
			Guard.IsNotNull(httpResponseCacher, "httpResponseCacher");
			Guard.IsNotNull(urlGenerator, "urlGenerator");
			this.siteMaps = siteMaps;
			this.httpResponseCompressor = httpResponseCompressor;
			this.httpResponseCacher = httpResponseCacher;
			this.urlGenerator = urlGenerator;
		}
		public SiteMapHandler() : this(SiteMapManager.SiteMaps, DI.Current.Resolve<IHttpResponseCompressor>(), DI.Current.Resolve<IHttpResponseCacher>(), DI.Current.Resolve<IUrlGenerator>())
		{
		}
		public override void ProcessRequest(HttpContextBase context)
		{
			string name = context.Request.QueryString["name"];
			SiteMapBase siteMap = this.GetSiteMap(name);
			if (siteMap != null && siteMap.GenerateSearchEngineMap)
			{
				HttpResponseBase response = context.Response;
				response.ContentType = "text/xml";
				if (siteMap.Compress)
				{
					this.httpResponseCompressor.Compress(context);
				}
				using (StreamWriter streamWriter = new StreamWriter(response.OutputStream, Encoding.UTF8))
				{
					using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter, new XmlWriterSettings
					{
						Indent = false,
						Encoding = Encoding.UTF8
					}))
					{
						this.WriteSiteMap(xmlWriter, siteMap, context);
					}
				}
				this.httpResponseCacher.Cache(context, TimeSpan.FromMinutes((double)siteMap.CacheDurationInMinutes));
			}
		}
		private static string GetPriority(SiteMapNode node)
		{
			int updatePriority = (int)node.UpdatePriority;
			return ((double)updatePriority * 0.01).ToString("0.0", CultureInfo.InvariantCulture);
		}
		private void WriteSiteMap(XmlWriter writer, SiteMapBase siteMap, HttpContextBase httpContext)
		{
			string applicationRoot = httpContext.Request.ApplicationRoot();
			writer.WriteStartDocument();
			writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
			this.duplicateChecks.Clear();
			this.WriteNode(writer, siteMap.RootNode, httpContext, applicationRoot);
			siteMap.RootNode.ChildNodes.Each(delegate(SiteMapNode node)
			{
				this.Iterate(writer, node, httpContext, applicationRoot);
			});
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}
		private void Iterate(XmlWriter writer, SiteMapNode node, HttpContextBase httpContext, string applicationRoot)
		{
			this.WriteNode(writer, node, httpContext, applicationRoot);
			node.ChildNodes.Each(delegate(SiteMapNode childNode)
			{
				this.Iterate(writer, childNode, httpContext, applicationRoot);
			});
		}
		private void WriteNode(XmlWriter writer, SiteMapNode node, HttpContextBase httpContext, string applicationRoot)
		{
			if (node.IncludeInSearchEngineIndex)
			{
				string url = this.GetUrl(node, httpContext, applicationRoot);
				if (!string.IsNullOrEmpty(url) && !this.duplicateChecks.ContainsKey(url))
				{
					writer.WriteStartElement("url", "http://www.sitemaps.org/schemas/sitemap/0.9");
					writer.WriteElementString("loc", "http://www.sitemaps.org/schemas/sitemap/0.9", url);
					if (node.LastModifiedAt.HasValue)
					{
						writer.WriteElementString("lastmod", "http://www.sitemaps.org/schemas/sitemap/0.9", node.LastModifiedAt.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
					}
					if (node.ChangeFrequency != SiteMapChangeFrequency.Automatic)
					{
						writer.WriteElementString("changefreq", "http://www.sitemaps.org/schemas/sitemap/0.9", node.ChangeFrequency.ToString().ToLowerInvariant());
					}
					if (node.UpdatePriority != SiteMapUpdatePriority.Automatic)
					{
						string priority = SiteMapHandler.GetPriority(node);
						writer.WriteElementString("priority", "http://www.sitemaps.org/schemas/sitemap/0.9", priority);
					}
					writer.WriteEndElement();
					this.duplicateChecks.Add(url, url);
				}
			}
		}
		private string GetUrl(INavigatable node, HttpContextBase httpContext, string applicationRoot)
		{
			string text = this.urlGenerator.Generate(httpContext.RequestContext(), node);
			if (!string.IsNullOrEmpty(text))
			{
				if (!text.StartsWith("/", StringComparison.Ordinal))
				{
					text = "/" + text;
				}
				text = applicationRoot + text;
			}
			return text;
		}
		private SiteMapBase GetSiteMap(string name)
		{
			SiteMapBase defaultSiteMap;
			if (string.IsNullOrEmpty(name))
			{
				defaultSiteMap = this.siteMaps.DefaultSiteMap;
			}
			else
			{
				this.siteMaps.TryGetValue(name, out defaultSiteMap);
			}
			return defaultSiteMap;
		}
	}
}
