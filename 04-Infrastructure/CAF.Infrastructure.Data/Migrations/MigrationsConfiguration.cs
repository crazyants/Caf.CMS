namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class MigrationsConfiguration : DbMigrationsConfiguration<DefaultObjectContext>
    {
        public MigrationsConfiguration()
        {

            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "CAF.Infrastructure.Core";
          
        }

        protected override void Seed(DefaultObjectContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //var log = new Log
            //{
            //    LogLevel = LogLevel.Error,
            //    ShortMessage = "ShortMessage1",
            //    FullMessage = "FullMessage1",
            //    IpAddress = "127.0.0.1",
            //    PageUrl = "http://www.someUrl1.com",
            //    ReferrerUrl = "http://www.someUrl2.com",
            //    CreatedOnUtc = new DateTime(2010, 01, 01)
            //};

            //context.Set<Log>().Add(log);
            //context.SaveChanges();
            //base.Seed(context);
        }
    }
}
