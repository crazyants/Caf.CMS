
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
namespace CAF.WebSite.CustomBanner.Models
{
    public class PictureEditTabModel : ModelBase
    {

        public int EntityId { get; set; }
        [LangResourceDisplayName("Admin.CustomBanner.Fields.PictureId")]
        [UIHint("Picture")]
        public int PictureId { get; set; }
        public string EntityName { get; set; }
    }
}
