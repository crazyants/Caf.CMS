 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace CAF.Infrastructure.Core.Logging
{
    public class LogReader
    {
        public static FileStream ReadFileStream(string filePath) 
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public static string ReadFile(string filePath)
        {
            string message = string.Empty;
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }
            return message;
        }

        public string ReadFile(string filePath, string level)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                    {
                        string pattern = @"\d{4}(-|\/)((0[1-9])|(1[0-2]))(-|\/)((0[1-9])|([1-2][0-9])|(3[0-1]))(\s)(([0-1][0-9])|(2[0-3])):([0-5][0-9]):([0-5][0-9])([,|\.]*[\d]*)(.*?)[\x20][[]?" + level + @"([]])?[\x20][\s\S]*";
                        string str2 = @"\d{4}(-|\/)((0[1-9])|(1[0-2]))(-|\/)((0[1-9])|([1-2][0-9])|(3[0-1]))(\s)(([0-1][0-9])|(2[0-3])):([0-5][0-9]):([0-5][0-9])[\s\S]+";
                        bool flag = false;
                        string input = string.Empty;
                        while (input != null)
                        {
                            input = reader.ReadLine();
                            if (input != null)
                            {
                                if (Regex.IsMatch(input, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                                {
                                    builder.AppendLine(input);
                                    flag = true;
                                }
                                else if (flag)
                                {
                                    if (Regex.IsMatch(input, str2, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                                    {
                                        flag = false;
                                        continue;
                                    }
                                    builder.AppendLine(input);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                return (exception.Message + " [StackTrace]" + exception.StackTrace);
            }
            return builder.ToString();
        }

        public static string ReadFile(string filePath, DateTime startTime, DateTime endTime,int page,int rows)
        {
            //StringBuilder builder = new StringBuilder();
            var result = new List<string>();
            //var lines = new List<string>();
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                    {
                        string pattern = @"\d{4}(-|\/)((0[1-9])|(1[0-2]))(-|\/)((0[1-9])|([1-2][0-9])|(3[0-1]))(\s)(([0-1][0-9])|(2[0-3])):([0-5][0-9]):([0-5][0-9])[\s\S]+";
                        bool flag = false;
                        string input = string.Empty;
                        while (input != null)
                        {
                            input = reader.ReadLine();
                            if (input != null)
                            {
                                if (Regex.IsMatch(input, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                                {
                                    DateTime time;
                                    if ((DateTime.TryParse(input.Substring(0, 0x13), out time) && (DateTime.Compare(time, startTime) >= 0)) && (DateTime.Compare(time, endTime) <= 0))
                                    {
                                        //builder.AppendLine(input);
                                        result.Add(input);
                                        flag = true;
                                    }
                                }
                                else if (flag)
                                {
                                    //builder.AppendLine(input);
                                    result.Add(input);
                                }
                            }
                        }
                    }
                }

                if (rows > 0)
                {
                    if (page == 0) page = 1;
                    result = (from line in result orderby line descending select line).Skip((page - 1) * rows).Take(rows).ToList();
                }
            }
            catch (Exception exception)
            {
                return (exception.Message + " [StackTrace]" + exception.StackTrace);
            }
            //return builder.ToString();
            return String.Join("\r\n", result);
        }

        public string ReadFile(string filePath, DateTime startTime, DateTime endTime, string level)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                    {
                        string pattern = @"\d{4}(-|\/)((0[1-9])|(1[0-2]))(-|\/)((0[1-9])|([1-2][0-9])|(3[0-1]))(\s)(([0-1][0-9])|(2[0-3])):([0-5][0-9]):([0-5][0-9])([,|\.]*[\d]*)(.*?)[\x20][[]?" + level + @"([]])?[\x20][\s\S]*";
                        string str2 = @"\d{4}(-|\/)((0[1-9])|(1[0-2]))(-|\/)((0[1-9])|([1-2][0-9])|(3[0-1]))(\s)(([0-1][0-9])|(2[0-3])):([0-5][0-9]):([0-5][0-9])[\s\S]+";
                        bool flag = false;
                        string input = string.Empty;
                        while (input != null)
                        {
                            input = reader.ReadLine();
                            if (input != null)
                            {
                                if (Regex.IsMatch(input, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                                {
                                    DateTime time;
                                    if ((DateTime.TryParse(input.Substring(0, 0x13), out time) && (DateTime.Compare(time, startTime) >= 0)) && (DateTime.Compare(time, endTime) <= 0))
                                    {
                                        builder.AppendLine(input);
                                        flag = true;
                                    }
                                }
                                else if (flag)
                                {
                                    if (Regex.IsMatch(input, str2, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                                    {
                                        flag = false;
                                        continue;
                                    }
                                    builder.AppendLine(input);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                return (exception.Message + " [StackTrace]" + exception.StackTrace);
            }
            return builder.ToString();
        }
    }
}
