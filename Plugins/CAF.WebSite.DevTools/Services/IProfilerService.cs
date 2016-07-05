using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.DevTools.Services
{
	public interface IProfilerService
	{
		void StepStart(string key, string message);
		void StepStop(string key);
	}
}