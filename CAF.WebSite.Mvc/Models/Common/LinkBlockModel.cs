
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Media;
using System.Collections.Generic;
namespace CAF.WebSite.Mvc.Models.Common
{

    public partial class LinkModel : ModelBase
    {
        public LinkModel()
        {

            LinkBlocks = new List<LinkBlockModel>();
        }

        public IList<LinkBlockModel> LinkBlocks { get; set; }
    }
    public partial class LinkBlockModel : ModelBase
    {
        public string Name { get; set; }

        public string Intro { get; set; }

        public string LinkUrl { get; set; }

        public string LogoUrl { get; set; }

        public int SortId { get; set; }

        public bool IsHome { get; set; }

        public PictureModel DefaultPictureModel { get; set; }
    }
}