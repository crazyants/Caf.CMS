using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Media;
using System.Collections.Generic;

namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    public partial class ClientModel : EntityModelBase
    {
        public ClientModel()
        {
            PictureModel = new PictureModel();

        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool IsActive { get; set; }
        public PictureModel PictureModel { get; set; }


    }
}