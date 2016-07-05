using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Media;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Models.Catalog
{
    public partial class ManufacturerOverviewModel : EntityModelBase
    {
        public ManufacturerOverviewModel()
        {
            PictureModel = new PictureModel();
        }

        public string Name { get; set; }
        public string SeName { get; set; }
        public string Description { get; set; }
        
        //picture
        public PictureModel PictureModel { get; set; }
    }
}