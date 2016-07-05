namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SQLProfilerLog", "ErrorId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SQLProfilerLog", "ErrorId", c => c.Int(nullable: false));
        }
    }
}
