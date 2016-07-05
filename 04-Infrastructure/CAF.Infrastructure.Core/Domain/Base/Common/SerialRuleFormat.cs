namespace CAF.Infrastructure.Core.Domain.Common
{
    /// <summary>
    /// Represents the user name fortatting enumeration
    /// </summary>
    public enum SerialRuleFormat : int
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 1,
        /// <summary>
        ///日期
        /// </summary>
        Data = 2,
        /// <summary>
        /// 随机数
        /// </summary>
        Rank=3
    }
}
