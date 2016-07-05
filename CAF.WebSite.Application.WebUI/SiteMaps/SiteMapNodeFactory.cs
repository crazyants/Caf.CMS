using System;
using Telerik.Web.Mvc.Infrastructure;
namespace CAF.WebSite.Application.WebUI
{
	public class SiteMapNodeFactory : IHideObjectMembers
	{
		private readonly SiteMapNode parent;
		public SiteMapNodeFactory(SiteMapNode parent)
		{
			Guard.IsNotNull(parent, "parent");
			this.parent = parent;
		}
		public SiteMapNodeBuilder Add()
		{
			SiteMapNode siteMapNode = new SiteMapNode();
			this.parent.ChildNodes.Add(siteMapNode);
			return new SiteMapNodeBuilder(siteMapNode);
		}
		Type IHideObjectMembers.GetType()
		{
			return base.GetType();
		}
	}
}
