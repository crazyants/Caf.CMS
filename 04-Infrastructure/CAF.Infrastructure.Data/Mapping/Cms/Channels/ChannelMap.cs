
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CAF.Infrastructure.Data.Mapping.Cms.Channels
{
    public partial class ChannelMap : EntityTypeConfiguration<Channel>
    {
        public ChannelMap()
        {
            this.ToTable("Channel");
            this.HasKey(a => a.Id);
            this.Property(a => a.Name).IsRequired().HasMaxLength(50);
            this.Property(a => a.Title).HasMaxLength(100);
            this.HasMany(p => p.ExtendedAttributes)
             .WithMany(pt => pt.Channels)
             .Map(m => m.ToTable("Channel_ExtendedAttribute_Mapping"));

        }
    }
}
