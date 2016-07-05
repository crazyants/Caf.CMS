using CAF.Infrastructure.Core;
using System;
namespace CAF.WebSite.CustomBanner.Domain
{
    public class CustomBannerRecord : AuditedBaseEntity
	{
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public int PictureId { get; set; }
		 
	}
}
