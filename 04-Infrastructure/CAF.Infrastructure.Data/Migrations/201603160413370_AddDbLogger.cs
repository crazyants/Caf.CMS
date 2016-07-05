namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDbLogger : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SQLProfilerLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ErrorId = c.Int(nullable: false),
                        Query = c.String(),
                        Parameters = c.String(),
                        CommandType = c.String(maxLength: 1000),
                        TotalSeconds = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Exception = c.String(),
                        InnerException = c.String(),
                        RequestId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 100),
                        CreateDate = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaxCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TaxCategory");
            DropTable("dbo.SQLProfilerLog");
        }
    }
}
