using CAF.Infrastructure.Core.Collections;
using CAF.WebSite.Application.WebUI;
using System;
using System.Collections.Generic;
 

namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    public class NavigationModelBuiltEvent
    {
        public NavigationModelBuiltEvent(TreeNode<MenuItem> rootNode)
        {
            this.RootNode = rootNode;
        }

		public TreeNode<MenuItem> RootNode { get; private set; }
    }
}