namespace CAF.WebSite.CustomBanner.Data.Migrations
{
    using CAF.Infrastructure.Data.Setup;
    using System;
    using System.Data.Entity.Migrations;

    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            if (DbMigrationContext.Current.SuppressInitialCreate<CustomBannerObjectContext>())
				return;
			
			CreateTable(
                "dbo.CustomBanner",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(maxLength: 400),
                        PictureId = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(nullable: false),
                        ModifiedUserID = c.Long(nullable: false),
                        ModifiedOnUtc = c.DateTime(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion")
                       
                    })
                .PrimaryKey(t => t.Id);
                
        }
        
        public override void Down()
        {
            DropTable("dbo.CustomBanner");
        }
    }
}
