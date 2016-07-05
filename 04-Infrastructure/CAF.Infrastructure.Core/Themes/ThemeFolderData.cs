using CAF.Infrastructure.Core.Collections;
using System.Xml;

namespace CAF.Infrastructure.Core.Themes
{
	internal class ThemeFolderData : ITopologicSortable<string>
	{
		public string FolderName { get; set; }
		public string FullPath { get; set; }
		public string VirtualBasePath { get; set; }
		public XmlDocument Configuration { get; set; }
		public string BaseTheme { get; set; }

		string ITopologicSortable<string>.Key
		{
			get { return FolderName; }
		}

		string[] ITopologicSortable<string>.DependsOn
		{
			get
			{
				if (BaseTheme == null)
					return null;

				return new [] { BaseTheme };
			}
		}
	}
}
