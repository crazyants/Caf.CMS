
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.RegionalContents;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Data.Mapping.Cms.RegionalContents
{

    public class RegionalContentMap : EntityTypeConfiguration<RegionalContent>
    {
        public RegionalContentMap()
        {
            this.ToTable("RegionalContent");
            this.HasKey(t => t.Id);
            this.Property(p => p.Name).HasMaxLength(100);
            this.Property(p => p.SystemName).IsRequired().HasMaxLength(40);
            this.Property(t => t.Body).IsMaxLength();
        }
    }
}
