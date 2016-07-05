using System;

namespace CAF.Infrastructure.Core
{
	public interface IActivatable
	{
		bool IsActive { get; }
	}
}
