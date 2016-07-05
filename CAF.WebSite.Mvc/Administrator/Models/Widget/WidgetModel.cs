


using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Admin.Models.Widget
{
	public class WidgetModel : ProviderModel, IActivatable
    {
        [LangResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

    }
}