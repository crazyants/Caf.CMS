using System;

namespace CAF.Infrastructure.Core.Data
{
	public interface ITransaction : IDisposable
	{
		void Commit();
		void Rollback();
	}
}
