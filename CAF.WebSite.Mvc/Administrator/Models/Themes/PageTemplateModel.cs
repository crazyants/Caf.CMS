using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Mvc.Admin.Models.Themes
{
    public class PageTemplateModel
    {
        public PageTemplateModel()
        {
            CssFiles = new List<CssFileViewModel>();
            ViewFiles = new List<ViewFileViewModel>();
        }

        public string Name { get; set; }
        public int SiteId { get; set; }
        public List<CssFileViewModel> CssFiles { get; set; }
        public List<ViewFileViewModel> ViewFiles { get; set; }
    }

    public class CssFileViewModel
    {
        public string Name { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ViewFileViewModel
    {
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}