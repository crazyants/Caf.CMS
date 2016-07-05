
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.WebUI.Localization
{
	public class Text : IText
	{
		private readonly ILocalizationService _localizationService;

		public Text(ILocalizationService localizationService)
		{
			this._localizationService = localizationService;
		}
		
		public LocalizedString Get(string key, params object[] args)
		{
			var value = _localizationService.GetResource(key);
			if (string.IsNullOrEmpty(value))
			{
				return new LocalizedString(key);
			}
			return
				new LocalizedString((args == null || args.Length == 0)
										? value
										: string.Format(value, args), key, args);
		}
	}
}
