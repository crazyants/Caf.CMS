using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;

namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    public partial class ClientNavigationModel : ModelBase
    {
        public ClientNavigationModel()
        {
            this.Clients = new List<ClientModel>();
        }

        public IList<ClientModel> Clients { get; set; }

        public int TotalClients { get; set; }
    }
 
}