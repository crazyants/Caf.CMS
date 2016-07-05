using CAF.Infrastructure.Core;
using System;

namespace CAF.WebSite.Application.WebUI.Events
{
	public class ThemeTouched
	{
		public ThemeTouched(string themeName)
		{
			Guard.ArgumentNotEmpty(() => themeName);
			this.ThemeName = themeName;
		}
		
		public string ThemeName { get; set; }
	}
}
