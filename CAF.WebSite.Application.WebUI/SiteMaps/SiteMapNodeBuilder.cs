using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace CAF.WebSite.Application.WebUI
{
	public class SiteMapNodeBuilder : IHideObjectMembers
	{
		private readonly SiteMapNode siteMapNode;
		public SiteMapNodeBuilder(SiteMapNode siteMapNode)
		{
		
			this.siteMapNode = siteMapNode;
		}
		public static implicit operator SiteMapNode(SiteMapNodeBuilder builder)
		{
		
			return builder.ToNode();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SiteMapNode ToNode()
		{
			return this.siteMapNode;
		}
		public virtual SiteMapNodeBuilder Title(string value)
		{
			this.siteMapNode.Title = value;
			return this;
		}
		public virtual SiteMapNodeBuilder Visible(bool value)
		{
			this.siteMapNode.Visible = value;
			return this;
		}
		public virtual SiteMapNodeBuilder LastModifiedAt(DateTime value)
		{
			this.siteMapNode.LastModifiedAt = new DateTime?(value);
			return this;
		}
		public virtual SiteMapNodeBuilder Route(string routeName, RouteValueDictionary routeValues)
		{
			this.siteMapNode.RouteName = routeName;
			this.SetRouteValues(routeValues);
			this.SetTitleIfEmpty(routeName);
			return this;
		}
		public virtual SiteMapNodeBuilder Route(string routeName, object routeValues)
		{
			this.Route(routeName, null);
			this.SetRouteValues(routeValues);
			return this;
		}
		public virtual SiteMapNodeBuilder Route(string routeName)
		{
			return this.Route(routeName, null);
		}
		public SiteMapNodeBuilder Action(RouteValueDictionary routeValues)
		{
			this.siteMapNode.Action(routeValues);
			return this;
		}
		public virtual SiteMapNodeBuilder Action(string actionName, string controllerName, RouteValueDictionary routeValues)
		{
			this.siteMapNode.ControllerName = controllerName;
			this.siteMapNode.ActionName = actionName;
			this.SetRouteValues(routeValues);
			this.SetTitleIfEmpty(actionName);
			return this;
		}
		public virtual SiteMapNodeBuilder Action(string actionName, string controllerName, object routeValues)
		{
			this.Action(actionName, controllerName, null);
			this.SetRouteValues(routeValues);
			return this;
		}
		public virtual SiteMapNodeBuilder Action(string actionName, string controllerName)
		{
			return this.Action(actionName, controllerName, null);
		}
		public virtual SiteMapNodeBuilder Action<TController>(Expression<Action<TController>> controllerAction) where TController : Controller
		{
			MethodCallExpression methodCallExpression = (MethodCallExpression)controllerAction.Body;
			string text = typeof(TController).Name;
			if (!text.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(TextResource.ControllerNameMustEndWithController, "controllerAction");
			}
			text = text.Substring(0, text.Length - "Controller".Length);
			if (text.Length == 0)
			{
				throw new ArgumentException(TextResource.CannotRouteToClassNamedController, "controllerAction");
			}
			if (methodCallExpression.Method.IsDefined(typeof(NonActionAttribute), false))
			{
				throw new ArgumentException(TextResource.TheSpecifiedMethodIsNotAnActionMethod, "controllerAction");
			}
			string actionName = (
				from attribute in methodCallExpression.Method.GetCustomAttributes(typeof(ActionNameAttribute), false).OfType<ActionNameAttribute>()
				select attribute.Name).FirstOrDefault<string>() ?? methodCallExpression.Method.Name;
			this.siteMapNode.ControllerName = text;
			this.siteMapNode.ActionName = actionName;
			ParameterInfo[] parameters = methodCallExpression.Method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				Expression expression = methodCallExpression.Arguments[i];
				ConstantExpression constantExpression = expression as ConstantExpression;
				object value;
				if (constantExpression != null)
				{
					value = constantExpression.Value;
				}
				else
				{
					Expression<Func<object>> expression2 = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object)), new ParameterExpression[0]);
					Func<object> func = expression2.Compile();
					value = func();
				}
				this.siteMapNode.RouteValues.Add(parameters[i].Name, value);
			}
			return this;
		}
		public virtual SiteMapNodeBuilder Url(string value)
		{
			this.siteMapNode.Url = value;
			return this;
		}
		public virtual SiteMapNodeBuilder ChangeFrequency(SiteMapChangeFrequency value)
		{
			this.siteMapNode.ChangeFrequency = value;
			return this;
		}
		public virtual SiteMapNodeBuilder UpdatePriority(SiteMapUpdatePriority value)
		{
			this.siteMapNode.UpdatePriority = value;
			return this;
		}
		public virtual SiteMapNodeBuilder IncludeInSearchEngineIndex(bool value)
		{
			this.siteMapNode.IncludeInSearchEngineIndex = value;
			return this;
		}
		public virtual SiteMapNodeBuilder Attributes(IDictionary<string, object> value)
		{
			
			this.siteMapNode.Attributes.Clear();
			this.siteMapNode.Attributes.Merge(value);
			return this;
		}
		public virtual SiteMapNodeBuilder Attributes(object value)
		{
			
			return this.Attributes(new RouteValueDictionary(value));
		}
		public virtual SiteMapNodeBuilder ChildNodes(Action<SiteMapNodeFactory> addActions)
		{
			
			SiteMapNodeFactory obj = new SiteMapNodeFactory(this.siteMapNode);
			addActions(obj);
			return this;
		}
		private void SetRouteValues(ICollection<KeyValuePair<string, object>> values)
		{
			if (values != null && values.Any<KeyValuePair<string, object>>())
			{
				this.siteMapNode.RouteValues.Clear();
				this.siteMapNode.RouteValues.AddRange(values);
			}
		}
		private void SetRouteValues(object values)
		{
			if (values != null)
			{
				this.siteMapNode.RouteValues.Clear();
				this.siteMapNode.RouteValues.Merge(values);
			}
		}
		private void SetTitleIfEmpty(string value)
		{
			if (string.IsNullOrEmpty(this.siteMapNode.Title))
			{
				this.siteMapNode.Title = value;
			}
		}

	}
}
