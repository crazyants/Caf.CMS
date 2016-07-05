using CAF.Infrastructure.Core.Domain.Logging;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;

 

namespace CAF.Infrastructure.Core.Logging
{
	public class LogContext
	{
		public string ShortMessage { get; set; }
		public string FullMessage { get; set; }
		public LogLevel LogLevel { get; set; }
        public User User { get; set; }

		public bool HashNotFullMessage { get; set; }
		public bool HashIpAddress { get; set; }
	}
}
