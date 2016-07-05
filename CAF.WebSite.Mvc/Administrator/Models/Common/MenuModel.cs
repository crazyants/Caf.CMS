using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Mvc.Admin.Models.Common
{

    public class MenuRoot
    {
        public MenuRoot()
        {
            menu = new List<MenuModel>();
        }
        public string id { get; set; }
        public List<MenuModel> menu { get; set; }


    }

    public class MenuModel
    {
        public MenuModel()
        {
            items = new List<MenuItemModel>();
        }
        public string text { get; set; }
        public List<MenuItemModel> items { get; set; }
    }

    public class MenuItemModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public string href { get; set; }

    }
}