using CAF.Infrastructure.Core.Domain.Themes;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
 

namespace CAF.Infrastructure.Data.Mapping.Themes
{
    
    public class ThemeVariableMap : EntityTypeConfiguration<ThemeVariable>
    {
        public ThemeVariableMap()
        { 
            this.ToTable("ThemeVariable");
            this.HasKey(t => t.Id);
            this.Property(t => t.Theme).HasMaxLength(400);
            this.Property(t => t.Name).HasMaxLength(400);
            this.Property(t => t.Value).HasMaxLength(2000).IsOptional();
        }
    }

}
