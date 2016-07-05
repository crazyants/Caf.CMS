using CAF.Infrastructure.Core.Collections;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CAF.WebSite.Mvc.Admin.Models.Plugins
{

    public class LocalPluginsModel : ModelBase
    {

        public LocalPluginsModel()
        {
            this.Groups = new Multimap<string, PluginModel>();
        }

		public List<SiteModel> AvailableSites { get; set; }

        public Multimap<string, PluginModel> Groups { get; set; }

        public ICollection<PluginModel> AllPlugins
        {
            get
            {
                return Groups.SelectMany(k => k.Value).ToList().AsReadOnly();
            }
        }

    }

}