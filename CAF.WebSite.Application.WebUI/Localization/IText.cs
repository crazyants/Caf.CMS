using System;
using CAF.Infrastructure.Core.Localization;

namespace CAF.WebSite.Application.WebUI.Localization
{
	public interface IText
	{
		LocalizedString Get(string key, params object[] args);
	}
}
