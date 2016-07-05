
using CAF.Infrastructure.Core.Configuration;
namespace CAF.Infrastructure.Core.Domain.Common
{
    public class AdminAreaSettings : ISettings
    {
		public AdminAreaSettings()
		{
			GridPageSize = 25;
            DisplayArticlePictures = true;
			RichEditorFlavor = "RichEditor";
		}
		
		public int GridPageSize { get; set; }

        public bool DisplayArticlePictures { get; set; }

        public string RichEditorFlavor { get; set; }
    }
}