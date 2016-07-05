using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Routing;
namespace CAF.WebSite.Application.WebUI
{
	public class SiteMapNode : LinkedObjectBase<SiteMapNode>
	{
		private string title;
		private string routeName;
		private string controllerName;
		private string actionName;
		private string url;
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				
				this.title = value;
			}
		}
		public bool Visible
		{
			get;
			set;
		}
		public DateTime? LastModifiedAt
		{
			get;
			set;
		}
		public string RouteName
		{
			get
			{
				return this.routeName;
			}
			set
			{
				
				this.routeName = value;
				this.controllerName = (this.actionName = (this.url = null));
			}
		}
		public string ControllerName
		{
			get
			{
				return this.controllerName;
			}
			set
			{
				
				this.controllerName = value;
				this.routeName = (this.url = null);
			}
		}
		public string ActionName
		{
			get
			{
				return this.actionName;
			}
			set
			{
			
				this.actionName = value;
				this.routeName = (this.url = null);
			}
		}
		public RouteValueDictionary RouteValues
		{
			get;
			private set;
		}
		public string Url
		{
			get
			{
				return this.url;
			}
			set
			{
			
				this.url = value;
				this.routeName = (this.controllerName = (this.actionName = null));
				this.RouteValues.Clear();
			}
		}
		 
		public bool IncludeInSearchEngineIndex
		{
			get;
			set;
		}
		public IDictionary<string, object> Attributes
		{
			get;
			private set;
		}
		public IList<SiteMapNode> ChildNodes
		{
			get;
			private set;
		}
		public SiteMapNode()
		{
			this.Visible = true;
			this.RouteValues = new RouteValueDictionary();
			this.IncludeInSearchEngineIndex = true;
			this.Attributes = new RouteValueDictionary();
			this.ChildNodes = new LinkedObjectCollection<SiteMapNode>(this);
		}
		 
	}
}
