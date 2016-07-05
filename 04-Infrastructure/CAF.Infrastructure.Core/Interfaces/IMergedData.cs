using System.Collections.Generic;

namespace CAF.Infrastructure.Core
{
	public interface IMergedData
	{
		bool MergedDataIgnore { get; set; }
		Dictionary<string, object> MergedDataValues { get; }
	}
}
