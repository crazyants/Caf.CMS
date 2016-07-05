using System;
using System.Linq;
using System.Collections.Generic;

namespace CAF.Infrastructure.Core.Domain.Sites
{
    public static class SiteExtensions
	{
		/// <summary>
		/// Parse comma-separated Hosts
		/// </summary>
		/// <param name="site">Site</param>
		/// <returns>Comma-separated hosts</returns>
		public static string[] ParseHostValues(this Site site)
		{
			if (site == null)
				throw new ArgumentNullException("site");

			var parsedValues = new List<string>();
			if (!String.IsNullOrEmpty(site.Hosts))
			{
				string[] hosts = site.Hosts.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string host in hosts)
				{
					var tmp = host.Trim();
					if (!String.IsNullOrEmpty(tmp))
						parsedValues.Add(tmp);
				}
			}
			return parsedValues.ToArray();
		}

		/// <summary>
		/// Indicates whether a site contains a specified host
		/// </summary>
		/// <param name="site">Site</param>
		/// <param name="host">Host</param>
		/// <returns>true - contains, false - no</returns>
		public static bool ContainsHostValue(this Site site, string host)
		{
			if (site == null)
				throw new ArgumentNullException("site");

			if (String.IsNullOrEmpty(host))
				return false;

			var contains = site.ParseHostValues()
								.FirstOrDefault(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase)) != null;
			return contains;
		}
	}
}
