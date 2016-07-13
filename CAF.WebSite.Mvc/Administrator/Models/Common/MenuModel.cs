using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Mvc.Admin.Models.Common
{
    public class MenuModel
    {
        public MenuModel()
        {
            Childitems = new List<MenuModel>();
        }
        public string Id { get; set; }
        public string Text { get; set; }
        public string Href { get; set; }
        public List<MenuModel> Childitems { get; set; }
    }
 
}