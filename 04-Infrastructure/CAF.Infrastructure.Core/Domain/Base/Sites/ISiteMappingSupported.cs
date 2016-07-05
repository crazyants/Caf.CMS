namespace CAF.Infrastructure.Core.Domain.Sites
{
	/// <summary>
	/// Represents an entity which supports store mapping
	/// </summary>
	public partial interface ISiteMappingSupported
	{
		/// <summary>
		/// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
		/// </summary>
        bool LimitedToSites { get; set; }
	}
}
