namespace CAF.Infrastructure.Core.Domain.Users
{

    /// <summary>
    /// Represents a best customer report line
    /// </summary>
    public partial class BestUserReportLine
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the order count
        /// </summary>
        public int OrderCount { get; set; }
    }
}
