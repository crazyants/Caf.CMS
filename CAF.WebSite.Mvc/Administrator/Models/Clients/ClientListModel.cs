using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Clients
{
    public class ClientListModel : ModelBase
    {
        public ClientListModel()
        {
 
            BatchCategory = new BatchCategoryModel();
        }
        [LangResourceDisplayName("Admin.Catalog.Clients.List.SearchClientName")]
        [AllowHtml]
        public string SearchClientName { get; set; }

        public List<ClientModel> Clients { get; set; }
        public BatchCategoryModel BatchCategory { get; set; }
        public class BatchCategoryModel : ModelBase
        {
            public bool OpenTemplateCheckBox { get; set; }
          

            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.SelectedIds")]
            [AllowHtml]
            public ICollection<int> SelectedIds { get; set; }
            [LangResourceDisplayName("Admin.ContentManagement.Articles.List.TemplateId")]
            [AllowHtml]
            public int? TemplateId { get; set; }
            
        }
    }
}