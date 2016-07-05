using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class FeedbackMap : EntityTypeConfiguration<Feedback>
    {
        public FeedbackMap()
        {
            this.ToTable("FeedbackMap");
            this.HasKey(a => a.Id);
            this.Property(a => a.Title).IsRequired().HasMaxLength(100);
            this.Property(a => a.Content).HasMaxLength(500);
            this.Property(a => a.UserName).HasMaxLength(50);
            this.Property(a => a.UserTel).HasMaxLength(50);
            this.Property(a => a.UserQQ).HasMaxLength(50);
            this.Property(a => a.UserEmail).HasMaxLength(50);
            this.Property(a => a.ReplyContent).HasMaxLength(500);
            this.Property(a => a.IPAddress).HasMaxLength(50);
        }
    }
}
