using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Seedwork.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class TierPriceExtensions
    {
		/// <summary>
		/// Filter tier prices by a store
		/// </summary>
		/// <param name="source">Tier prices</param>
		/// <param name="siteId">Site identifier</param>
		/// <returns>Filtered tier prices</returns>
        public static IEnumerable<TierPrice> FilterBySite(this IEnumerable<TierPrice> source, int siteId)
		{
			if (source == null)
				throw new ArgumentNullException("source");

            return source.Where(x => x.SiteId == 0 || x.SiteId == siteId);
		}

        /// <summary>
        /// Filter tier prices for a user
        /// </summary>
        /// <param name="source">Tier prices</param>
        /// <param name="user">User</param>
        /// <returns>Filtered tier prices</returns>
        public static IEnumerable<TierPrice> FilterForUser(this IEnumerable<TierPrice> source, User user)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            foreach (var tierPrice in source)
            {
                //check user role requirement
                if (tierPrice.UserRole != null)
                {
                    if (user == null)
                        continue;

                    var userRoles = user.UserRoles.Where(cr => cr.Active);
                    if (!userRoles.Any())
                        continue;

                    bool roleIsFound = false;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole == tierPrice.UserRole)
                            roleIsFound = true;
                    }

                    if (!roleIsFound)
                        continue;

                }

                yield return tierPrice;
            }

        }

        /// <summary>
        /// Remove duplicated quantities (leave only a tier price with minimum price)
        /// </summary>
        /// <param name="source">Tier prices</param>
        /// <returns>Filtered tier prices</returns>
        public static ICollection<TierPrice> RemoveDuplicatedQuantities(this ICollection<TierPrice> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            
            //find duplicates
            var query = from tierPrice in source
                        group tierPrice by tierPrice.Quantity into g
                        where g.Count() > 1
                        select new { Quantity = g.Key, TierPrices = g.ToList() };
            foreach (var item in query)
            {
                //find a tier price record with minimum price (we'll not remove it)
                var minTierPrice = item.TierPrices.Aggregate((tp1, tp2) => (tp1.Price < tp2.Price ? tp1 : tp2));
                //remove all other records
                item.TierPrices.Remove(minTierPrice);
                item.TierPrices.ForEach(x=> source.Remove(x));
            }

            return source;
        }
    }
}
