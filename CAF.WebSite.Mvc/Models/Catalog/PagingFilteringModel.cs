using CAF.Infrastructure.Core.Pages;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
// codehint: sm-delete
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Models.Catalog
{

    // codehint: sm-add (whole file)

    public partial class ListOptionItem : ModelBase
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public bool Selected { get; set; }
        public object ExtraData { get; set; }

        public SelectListItem ToSelectListItem()
        {
            return new SelectListItem
            {
                Text = this.Text,
                Value = this.Url,
                Selected = this.Selected
            };
        }
    }

    // codehint: sm-add
    public partial class PagingFilteringModel : PagedListBase
    {
        
        public PagingFilteringModel()
        {
            this.AvailableSortOptions = new List<ListOptionItem>();
            this.AvailableViewModes = new List<ListOptionItem>();
            this.PageSizeOptions = new List<ListOptionItem>();
        }

        public bool AllowSorting { get; set; }
        public IList<ListOptionItem> AvailableSortOptions { get; set; }

        public bool AllowViewModeChanging { get; set; }
        public IList<ListOptionItem> AvailableViewModes { get; set; }

        public bool AllowUsersToSelectPageSize { get; set; }
        public IList<ListOptionItem> PageSizeOptions { get; set; }

        /// <summary>
        /// Order by
        /// </summary>
        [LangResourceDisplayName("Categories.OrderBy")]
        public int OrderBy { get; set; }

        /// <summary>
        /// Product sorting
        /// </summary>
        [LangResourceDisplayName("Categories.ViewMode")]
        public string ViewMode { get; set; }

    }

}