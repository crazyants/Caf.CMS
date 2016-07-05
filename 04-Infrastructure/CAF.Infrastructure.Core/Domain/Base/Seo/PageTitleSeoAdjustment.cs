namespace CAF.Infrastructure.Core.Domain.Seo
{
    /// <summary>
    /// Represents an editor type
    /// </summary>
    public enum PageTitleSeoAdjustment
    {
        /// <summary>
        /// Pagename comes after storename
        /// </summary>
        PagenameAfterSitename = 0,
        /// <summary>
        /// Sitename comes after pagename
        /// </summary>
        SitenameAfterPagename = 10
    }
}
