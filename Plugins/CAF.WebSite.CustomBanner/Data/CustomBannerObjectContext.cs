using CAF.Infrastructure.Data;
using CAF.Infrastructure.Data.Setup;
using CAF.WebSite.CustomBanner.Data.Migrations;
using CAF.WebSite.CustomBanner.Domain;
using System;
using System.Data.Entity;
namespace CAF.WebSite.CustomBanner.Data
{
	public class CustomBannerObjectContext : ObjectContextBase
	{
        public const string ALIASKEY = "caf_object_context_custom_banner";
		static CustomBannerObjectContext()
		{
            var initializer = new MigrateDatabaseInitializer<CustomBannerObjectContext, Configuration>
            {
                TablesToCheck = new[] { "CustomBanner" }
            };
            Database.SetInitializer(initializer);
		}
		public CustomBannerObjectContext()
		{
		}
        public CustomBannerObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString, ALIASKEY)
		{
		}
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
            modelBuilder.Configurations.Add(new CustomBannerRecordMap());
			base.OnModelCreating(modelBuilder);
		}
	}
}
