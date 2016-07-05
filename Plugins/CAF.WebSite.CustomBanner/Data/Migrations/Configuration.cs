using System;
using System.Data.Entity.Migrations;
namespace CAF.WebSite.CustomBanner.Data.Migrations
{
	internal sealed class Configuration : DbMigrationsConfiguration<CustomBannerObjectContext>
	{
		public Configuration()
		{
            base.AutomaticMigrationsEnabled = false;
            base.MigrationsDirectory = "Data\\Migrations";
            base.ContextKey = "CAF.WebSite.CustomBanner";
		}
		protected override void Seed(CustomBannerObjectContext context)
		{
		}
	}
}
