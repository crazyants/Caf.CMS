using System;
using System.Collections.Generic;

namespace CAF.Infrastructure.Core
{
	public interface ISoftDeletable
	{
		bool Deleted { get; }
	}
}
