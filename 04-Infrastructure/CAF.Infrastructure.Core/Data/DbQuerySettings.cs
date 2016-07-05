using System;

namespace CAF.Infrastructure.Core.Data
{
	public class DbQuerySettings
	{
		private readonly static DbQuerySettings s_default = new DbQuerySettings(false, false);

		public DbQuerySettings(bool ignoreAcl, bool ignoreMultiStore)
		{
			this.IgnoreAcl = ignoreAcl;
            this.IgnoreMultiSite = ignoreMultiStore;
		}

		public bool IgnoreAcl { get; private set; }
        public bool IgnoreMultiSite { get; private set; }

		public static DbQuerySettings Default
		{
			get { return s_default; }
		}
	}
}
