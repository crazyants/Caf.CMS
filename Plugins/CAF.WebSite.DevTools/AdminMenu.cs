using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core.Collections;


namespace CAF.WebSite.DevTools
{
	public class AdminMenu : AdminMenuProvider
	{
		protected override void BuildMenuCore(TreeNode<MenuItem> pluginsNode)
		{
			var menuItem = new MenuItem().ToBuilder()
                .Id("DevTool")
				.Text("Developer Tools")
				.Icon("code")
				.Action("ConfigurePlugin", "Plugin", new { systemName = "CAF.WebSite.DevTools", area = "Admin" })
				.ToItem();

			pluginsNode.Prepend(menuItem);
		}
	}
}
