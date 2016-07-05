namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents the article sorting
    /// </summary>
    public enum ArticleSortingEnum
    {
        /// <summary>
        /// Position (display order)
        /// </summary>
        Position = 0,
        /// <summary>
        /// Name: A to Z
        /// </summary>
        NameAsc = 5,
        /// <summary>
        /// Name: Z to A
        /// </summary>
        NameDesc = 6,
        /// <summary>
        /// article creation date
        /// </summary>
        CreatedOn = 15, // eigentlich CreatedOnDesc (wegen Lokalisierung bleibt das aber so)
        /// <summary>
        /// article creation date
        /// </summary>
        CreatedOnAsc = 16 // codehint: sm-add
    }
}