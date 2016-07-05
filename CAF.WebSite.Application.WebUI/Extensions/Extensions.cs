using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Helpers;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Localization;
using CAF.WebSite.Application.WebUI.Localization;



namespace CAF.WebSite.Application.WebUI
{
    public static class Extensions
    {

        public static IEnumerable<T> PagedForCommand<T>(this IEnumerable<T> current, int PageIndex, int PageSize)
        {
            return current.Skip(PageIndex * PageSize).Take(PageSize);
        }
        public static bool IsEntityFrameworkProvider(this IQueryProvider provider)
        {
            return provider.GetType().FullName == "System.Data.Objects.ELinq.ObjectQueryProvider";
        }

        public static bool IsLinqToObjectsProvider(this IQueryProvider provider)
        {
            return provider.GetType().FullName.Contains("EnumerableQuery");
        }

        public static string FirstSortableProperty(this Type type)
        {
            PropertyInfo firstSortableProperty = type.GetProperties().Where(property => property.PropertyType.IsPredefinedType()).FirstOrDefault();

            if (firstSortableProperty == null)
            {
                throw new NotSupportedException("Cannot find property to sort by.");
            }

            return firstSortableProperty.Name;
        }

        internal static bool IsPredefinedType(this Type type)
        {
            return PredefinedTypes.Any(t => t == type);
        }

        public static readonly Type[] PredefinedTypes = {
            typeof(Object),
            typeof(Boolean),
            typeof(Char),
            typeof(String),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert)
        };



        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, bool markCurrentAsSelected = true) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("An Enumeration type is required.", "enumObj");

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         select new { ID = Convert.ToInt32(enumValue), Name = enumValue.GetLocalizedEnum(localizationService, workContext) };
            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            return new SelectList(values, "ID", "Name", selectedValue);
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source)
        {
            return RelativeFormat(source, string.Empty);
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="defaultFormat">Default format string (in case relative formatting is not applied)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source, string defaultFormat)
        {
            return RelativeFormat(source, false, defaultFormat);
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="convertToUserTime">A value indicating whether we should convet DateTime instance to user local time (in case relative formatting is not applied)</param>
        /// <param name="defaultFormat">Default format string (in case relative formatting is not applied)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source,
            bool convertToUserTime, string defaultFormat)
        {
            string result = "";
            var localizer = EngineContext.Current.Resolve<ILocalizationService>();

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - source.Ticks);
            double delta = ts.TotalSeconds;

            if (delta > 0)
            {
                if (delta < 60) // 60 (seconds)
                {
                    result = ts.Seconds == 1 ? localizer.GetResource("Time.OneSecondAgo") : String.Format(localizer.GetResource("Time.OneSecondAgo"), ts.Seconds);
                }
                else if (delta < 120) //2 (minutes) * 60 (seconds)
                {
                    result = localizer.GetResource("Time.OneMinuteAgo");
                }
                else if (delta < 2700) // 45 (minutes) * 60 (seconds)
                {
                    result = String.Format(localizer.GetResource("Time.MinutesAgo"), ts.Minutes);
                }
                else if (delta < 5400) // 90 (minutes) * 60 (seconds)
                {
                    result = localizer.GetResource("Time.OneHourAgo");
                }
                else if (delta < 86400) // 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    int hours = ts.Hours;
                    if (hours == 1)
                        hours = 2;
                    result = String.Format(localizer.GetResource("Time.HoursAgo"), hours);
                }
                else if (delta < 172800) // 48 (hours) * 60 (minutes) * 60 (seconds)
                {
                    result = localizer.GetResource("Time.Yesterday");
                }
                else if (delta < 2592000) // 30 (days) * 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    result = String.Format(localizer.GetResource("Time.DaysAgo"), ts.Days);
                }
                else if (delta < 31104000) // 12 (months) * 30 (days) * 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    result = months <= 1 ? localizer.GetResource("Time.OneMonthAgo") : String.Format(localizer.GetResource("Time.MonthsAgo"), months);
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    result = years <= 1 ? localizer.GetResource("Time.OneYearAgo") : String.Format(localizer.GetResource("Time.YearsAgo"), years);
                }
            }
            else
            {
                DateTime tmp1 = source;
                if (convertToUserTime)
                {
                    tmp1 = EngineContext.Current.Resolve<IDateTimeHelper>().ConvertToUserTime(tmp1, DateTimeKind.Utc);
                }
                //default formatting
                if (!String.IsNullOrEmpty(defaultFormat))
                {
                    result = tmp1.ToString(defaultFormat);
                }
                else
                {
                    result = tmp1.ToString();
                }
            }
            return result;
        }

        public static string Prettify(this TimeSpan ts)
        {
            Localizer T = EngineContext.Current.Resolve<IText>().Get;
            double seconds = ts.TotalSeconds;

            try
            {
                int secsTemp = Convert.ToInt32(seconds);
                string label = T("Time.SecondsAbbr");
                int remainder = 0;
                string remainderLabel = "";

                if (secsTemp > 59)
                {
                    remainder = secsTemp % 60;
                    secsTemp /= 60;
                    label = T("Time.MinutesAbbr");
                    remainderLabel = T("Time.SecondsAbbr");
                }

                if (secsTemp > 59)
                {
                    remainder = secsTemp % 60;
                    secsTemp /= 60;
                    label = (secsTemp == 1) ? T("Time.HourAbbr") : T("Time.HoursAbbr");
                    remainderLabel = T("Time.MinutesAbbr");
                }

                if (remainder == 0)
                {
                    return string.Format("{0:#,##0.#} {1}", secsTemp, label);
                }
                else
                {
                    return string.Format("{0:#,##0} {1} {2} {3}", secsTemp, label, remainder, remainderLabel);
                }
            }
            catch
            {
                return "(-)";
            }
        }

        /// <summary>
        /// Get a list of all stores
        /// </summary>
        /// <remarks>codehint: caf-add</remarks>
        public static IList<SelectListItem> ToSelectListItems(this IEnumerable<Site> stores)
        {
            var lst = new List<SelectListItem>();

            foreach (var store in stores)
            {
                lst.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString()
                });
            }
            return lst;
        }

        public static void SelectValue(this List<SelectListItem> lst, string value, string defaultValue = null)
        {
            if (lst != null)
            {
                var itm = lst.FirstOrDefault(i => i.Value.IsCaseInsensitiveEqual(value));

                if (itm == null && defaultValue != null)
                    itm = lst.FirstOrDefault(i => i.Value.IsCaseInsensitiveEqual(defaultValue));

                if (itm != null)
                    itm.Selected = true;
            }
        }

        /// <summary>
        /// Determines whether a plugin is installed and activated for a particular store.
        /// </summary>
        public static bool IsPluginReady(this IPluginFinder pluginFinder, ISettingService settingService, string systemName, int storeId)
        {
            try
            {
                var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName(systemName);

                if (pluginDescriptor != null && pluginDescriptor.Installed)
                {
                    if (storeId == 0 || settingService.GetSettingByKey<string>(pluginDescriptor.GetSettingKey("LimitedToSites")).ToIntArrayContains(storeId, true))
                        return true;
                }
            }
            catch (Exception exc)
            {
                exc.Dump();
            }
            return false;
        }



    }
}
