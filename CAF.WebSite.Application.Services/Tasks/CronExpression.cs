using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCrontab;
using CronExpressionDescriptor;
using System.Threading;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Application.Services.Tasks
{
	/// <summary>
    /// Cron表达式 
    /// "* * * * *", // every Minute
    /// "*/10 * * * *", // Every 10 minutes
    /// "0 1 * * *", // At 01:00
    /// "0 */4 * * *", // Every 04 hours
    /// "0/15 * * * *", // Every 15 minutes
    /// "30 1,13 * * *", // At 01:30 and 13:30
    /// "0 2 * * *", // At 02:00
    /// "30 3 * * *", // At 03:30
	/// </summary>
	public static class CronExpression
	{

		public static bool IsValid(string expression)
		{
			try
			{
				CrontabSchedule.Parse(expression);
				return true;
			}
			catch { }

			return false;
		}

		public static DateTime GetNextSchedule(string expression, DateTime baseTime)
		{
			return GetFutureSchedules(expression, baseTime, 1).FirstOrDefault();
		}

		public static DateTime GetNextSchedule(string expression, DateTime baseTime, DateTime endTime)
		{
			return GetFutureSchedules(expression, baseTime, endTime, 1).FirstOrDefault();
		}

		public static IEnumerable<DateTime> GetFutureSchedules(string expression, DateTime baseTime, int max = 10)
		{
			return GetFutureSchedules(expression, baseTime, DateTime.MaxValue);
		}

		public static IEnumerable<DateTime> GetFutureSchedules(string expression, DateTime baseTime, DateTime endTime, int max = 10)
		{
			Guard.ArgumentNotEmpty(() => expression);

			var schedule = CrontabSchedule.Parse(expression);
			return schedule.GetNextOccurrences(baseTime, endTime).Take(max);
		}

		public static string GetFriendlyDescription(string expression)
		{
			Guard.ArgumentNotEmpty(() => expression);

			var options = new Options 
			{ 
				DayOfWeekStartIndexZero = true, 
				ThrowExceptionOnParseError = true, 
				Verbose = false,
				Use24HourTimeFormat = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AMDesignator.IsEmpty()
			};

			try
			{
				return ExpressionDescriptor.GetDescription(expression, options);
			}
			catch
			{
				return null;
			}
		}

	}

}
