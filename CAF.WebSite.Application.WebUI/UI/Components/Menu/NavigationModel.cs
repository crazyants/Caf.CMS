using CAF.Infrastructure.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.WebUI 
{
	
	public class NavigationModel
	{
		public TreeNode<MenuItem> Root { get; set; }
		public IList<MenuItem> Path { get; set; }

        public MenuItem CurrentMenu { get; set; }

		public MenuItem SelectedMenuItem
		{
			get
			{
				if (Path == null || Path.Count == 0)
					return null;

				return Path.Last();
			}
		}
	}

}
