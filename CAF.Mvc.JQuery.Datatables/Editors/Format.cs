// <copyright>Copyright (c) 2014 SpryMedia Ltd - All Rights Reserved</copyright>
//
// <summary>
// Formatter methods for Editor
// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Mvc.JQuery.Datatables.Core.Editors
{
    /// <summary>
    /// Formatter methods for the DataTables Editor. All of the methods in this
    /// class return a delegate that can be used in the <code>GetFormatter</code>
    /// and <code>SetFormatter</code> methods of the <code>Field</code> class.
    ///
    /// Each method may define its own parameters that configure how the
    /// formatter operates. For example the date / time formatters take information
    /// on the formatting to be used.
    /// </summary>
    public static class Format
    {
        /// <summary>
        /// Date format: 2012-03-09. jQuery UI equivalent format: yy-mm-dd
        /// </summary>
        public const string DATE_ISO_8601 = "yyyy-MM-dd";

        /// <summary>
        /// Date format: Fri, 9 Mar 12. jQuery UI equivalent format: D, d M y
        /// </summary>
        public const string DATE_ISO_822 = "ddd, d MMM yy";

        /// <summary>
        /// Date format: Friday, 09-Mar-12.  jQuery UI equivalent format: DD, dd-M-y
        /// </summary>
        public const string DATE_ISO_850 = "dddd, dd-MMM-yy";

        /// <summary>
        /// Date format: Fri, 9 Mar 12. jQuery UI equivalent format: D, d M y
        /// </summary>
        public const string DATE_ISO_1036 = "ddd, d MMM yy";

        /// <summary>
        /// Date format: Fri, 9 Mar 2012. jQuery UI equivalent format: D, d M yy
        /// </summary>
        public const string DATE_ISO_1123 = "ddd, d MMM yyyy";

        /// <summary>
        /// Date format: Fri, 9 Mar 2012. jQuery UI equivalent format: D, d M yy
        /// </summary>
        public const string DATE_ISO_2822 = "ddd, d MMM yyyy";



        /// <summary>
        /// Convert from SQL date / date time format (ISO8601) to a format given by the options parameter.
        /// </summary>
        /// <param name="format">Value to convert from SQL date format</param>
        /// <returns>Formatter delegate</returns>
        public static Func<dynamic, Dictionary<string, object>, dynamic> DateSqlToFormat(string format)
        {
            return Format.DateTime("yyyy-MM-dd", format);
        }

        /// <summary>
        /// Convert to SQL date / date time format (ISO8601) from a format given by the options parameter.
        /// </summary>
        /// <param name="format">Value to convert to SQL date format</param>
        /// <returns>Formatter delegate</returns>
        public static Func<dynamic, Dictionary<string, object>, dynamic> DateFormatToSql(string format)
        {
            return Format.DateTime(format, "yyyy-MM-dd");
        }

        /// <summary>
        /// Convert from one date time format to another
        /// </summary>
        /// <param name="from">From format</param>
        /// <param name="to">To format</param>
        /// <returns>Formatter delegate</returns>
        public static Func<dynamic, Dictionary<string, object>, dynamic> DateTime(string from, string to)
        {
            return delegate(dynamic val, Dictionary<string, object> data)
            {
                DateTime dt;

                if (val == null)
                {
                    return null;
                }
                else if (val is DateTime)
                {
                    dt = val;
                }
                else
                {
                    string str = Convert.ToString(val);

                    dt = System.DateTime.ParseExact(str, from, System.Globalization.CultureInfo.InvariantCulture);
                }

                return dt.ToString(to);
            };
        }

        /// <summary>
        /// Convert a string of values into an array for use with checkboxes.
        /// </summary>
        /// <param name="delimiter">Delimiter to split on</param>
        /// <returns>Formatter delegate</returns>
        public static Func<dynamic, Dictionary<string, object>, dynamic> Explode(string delimiter = "|")
        {
            return delegate(dynamic val, Dictionary<string, object> data)
            {
                string str = (string)val;
                return str.Split(new string[] { delimiter }, StringSplitOptions.None);
            };
        }

        /// <summary>
        /// Convert an array of values from a checkbox into a string which can be
        /// used to store in a text field in a database.
        /// </summary>
        /// <param name="delimiter">Delimiter to join on</param>
        /// <returns>Formatter delegate</returns>
        public static Func<dynamic, Dictionary<string, object>, dynamic> Implode(string delimiter = "|")
        {
            return delegate(dynamic val, Dictionary<string, object> data)
            {
                string[] str = (string[])val;
                return String.Join(delimiter, val);
            };
        }

        /// <summary>
        /// Convert an empty string to `null`. Null values are very useful in
        /// databases, but HTTP variables have no way of representing `null` as a
        /// value, often leading to an empty string and null overlapping. This method
        /// will check the value to operate on and return null if it is empty.
        /// </summary>
        /// <returns>Formatter delegate</returns>
        public static Func<dynamic, Dictionary<string, object>, dynamic> NullEmpty()
        {
            return delegate(dynamic val, Dictionary<string, object> data)
            {
                if ((string)val == "")
                {
                    return null;
                }
                return val;
            };
        }
    }
}
