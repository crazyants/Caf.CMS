using CAF.Infrastructure.Core.Domain.Cms.Topic;
using System.Data.Entity.ModelConfiguration;
 

namespace CAF.Infrastructure.Data.Mapping.Cms.Topics
{
    public class TopicMap : EntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            this.ToTable("Topic");
            this.HasKey(t => t.Id);
			this.Property(t => t.Body).IsMaxLength();
        }
    }
}
