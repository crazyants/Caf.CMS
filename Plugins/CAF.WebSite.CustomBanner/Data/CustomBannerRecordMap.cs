
using CAF.WebSite.CustomBanner.Domain;
using System;
using System.Data.Entity.ModelConfiguration;
namespace CAF.WebSite.CustomBanner.Data
{
    public class CustomBannerRecordMap : EntityTypeConfiguration<CustomBannerRecord>
    {
        public CustomBannerRecordMap()
        {
            base.ToTable("CustomBanner");
            base.HasKey(x => x.Id);
        }
    }
}
