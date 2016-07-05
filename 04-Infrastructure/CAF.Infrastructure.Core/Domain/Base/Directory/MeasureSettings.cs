
using CAF.Infrastructure.Core.Configuration;
namespace CAF.Infrastructure.Core.Domain.Directory
{
    public class MeasureSettings : ISettings
    {
        public int BaseDimensionId { get; set; }
        public int BaseWeightId { get; set; }
    }
}