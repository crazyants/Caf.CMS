using System;
using System.Collections.Generic;

namespace CAF.WebSite.Application.WebUI 
{
	[Flags]
	public enum NodePathState
	{
		Unknown = 0,
		Parent = 1,
		Expanded = 2,
		Selected = 4
	}
}
