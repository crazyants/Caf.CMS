using CAF.Infrastructure.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CAF.WebSite.Application.WebUI 
{
	public abstract class AdminMenuProvider : IMenuProvider
	{
		public void BuildMenu(TreeNode<MenuItem> rootNode)
		{
			var pluginsNode = rootNode.Children.FirstOrDefault(x => x.Value.Id == "plugins");
			BuildMenuCore(pluginsNode);
		}

		protected abstract void BuildMenuCore(TreeNode<MenuItem> pluginsNode);

		public string MenuName
		{
			get { return "admin"; }
		}

		public virtual int Ordinal
		{
			get { return 0; }
		}

	}
}
