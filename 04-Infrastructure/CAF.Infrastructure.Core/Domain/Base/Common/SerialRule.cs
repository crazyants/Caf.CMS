

using CAF.Infrastructure.Core;
namespace CAF.Infrastructure.Core.Domain.Common
{
    public class SerialRule : BaseEntity,ISoftDeletable
    {


        /// <summary>
        /// Gets or sets the facebook link
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the google plus link
        /// </summary>
        public string Name { get; set; }

        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the pinterest link
        /// </summary>
        public string Value { get; set; }

        public string DefaultValue { get; set; }

        public int RandNum { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// Gets or sets the twitter link
        /// </summary>
        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        public int SerialRuleFormatId { get; set; }

        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        public SerialRuleFormat SerialRuleFormat
        {
            get { return (SerialRuleFormat)SerialRuleFormatId; }
            set { this.SerialRuleFormatId = (int)value; }
        }




    }
}