using CAF.Infrastructure.Core;
using System.Runtime.Serialization;
namespace CAF.Infrastructure.Core.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// </summary>
	[DataContract]
	public partial class Setting : BaseEntity
    {
        public Setting() { }

        public Setting(string name, string value, int siteId = 0)
		{
            this.Name = name;
            this.Value = value;
            this.SiteId = siteId;
        }
        
        /// <summary>
        /// Gets or sets the name
        /// </summary>
		[DataMember]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
		[DataMember]
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the store for which this setting is valid. 0 is set when the setting is for all stores
		/// </summary>
		[DataMember]
		public int SiteId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
