using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AwesomeMvcDemo.Helpers
{
    public static class ShowCodeHelpers
    {
        /// <summary>
        /// get the string value of the controller code between the /*begin*/ and /*end*/ comment blocks  
        /// </summary>
        /// <param name="html"></param>
        /// <param name="path"> file location path</param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static IHtmlString Csrc(this HtmlHelper html, string path, object k = null)
        {
            var startWord = "/*begin" + k + "*/";
            var endWord = "/*end" + k + "*/";

            var newpath = html.ViewContext.HttpContext.Server.MapPath(@"~\" + path);
            var lines = System.IO.File.ReadAllLines(newpath);
            var code = GetCode(lines, startWord, endWord);
            return new MvcHtmlString(ParseStrToCode(code, path));
        }

        /// <summary>
        /// get the string value of the controller code between the /*begin(key)*/ and /*end(key)*/ comment blocks 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="path"></param>
        /// <param name="k">key</param>
        /// <returns></returns>
        public static IHtmlString Csource(this HtmlHelper html, string path, object k = null)
        {
            var startWord = "/*begin" + k + "*/";
            var endWord = "/*end" + k + "*/";

            var newpath = html.ViewContext.HttpContext.Server.MapPath(@"~\Controllers\" + path);
            var lines = System.IO.File.ReadAllLines(newpath);

            var code = GetCode(lines, startWord, endWord);

            return new MvcHtmlString(ParseStrToCode(code, path));
        }

        /// <summary>
        /// get the string value of js file between the /*begin*/ and /*end*/ comment blocks 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="path"></param>
        /// <param name="k">key</param>
        /// <returns></returns>
        public static IHtmlString SourceJs(this HtmlHelper html, string path, object k = null)
        {
            var startWord = "/*begin" + k + "*/";
            var endWord = "/*end" + k + "*/";
            
            var newpath = html.ViewContext.HttpContext.Server.MapPath(@"~\Scripts\" + path);
            var lines = System.IO.File.ReadAllLines(newpath);
            var code = GetCode(lines, startWord, endWord);

            return new MvcHtmlString(ParseStrToCode(code, path));
        }

        /// <summary>
        /// get the string value of the view code located between the begin+key and end+key comment blocks
        /// </summary>
        /// <param name="html"></param>
        /// <param name="path"></param>
        /// <param name="k">key</param>
        /// <param name="wrap">wrap with div class='code'</param>
        /// <returns></returns>
        public static IHtmlString Source(this HtmlHelper html, string path, object k = null, bool wrap = false)
        {
            var key = k == null ? "" : k.ToString();
            var newpath = html.ViewContext.HttpContext.Server.MapPath(@"~\Views\" + path);
            var lines = System.IO.File.ReadAllLines(newpath);
            
            
            var code = path.EndsWith(".cshtml") ? 
                GetCode(lines, "@*begin" + key + "*@", "@*end" + key + "*@"):
                GetCode(lines, "<%--begin" + key + "--%>", "<%--end" + key + "--%>");
            var result = ParseStrToCode(code, path);
            if (wrap) result = "<div class='code'>" + result + "</div>";

            return new MvcHtmlString(result);
        }

        private static string GetCode(string[] lines, string startWord, string endWord)
        {

            int starti = -1, endi = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                if (starti == -1)
                {
                    if (lines[i].Contains(startWord))
                    {
                        starti = i;
                    }
                }
                else
                {
                    if (lines[i].Contains(endWord))
                    {
                        endi = i;
                        break;
                    }
                }
            }

            if (starti != -1 && endi == -1) endi = lines.Length - 1;

            string[] res;
            if (endi == -1)
            {
                res = lines;
            }
            else
            {
                var alen = endi - (starti + 1);
                res = new string[alen];
                Array.Copy(lines, starti + 1, res, 0, alen);
            }

            int? minws = null;
            foreach (var line in res)
            {
                if (line.Length == 0) continue;

                var lmin = 0;
                foreach (var ch in line)
                {
                    if (ch == ' ') lmin++;
                    else break;
                }

                if (minws == null || lmin < minws) minws = lmin;
            }

            var sb = new StringBuilder();
            if (res.Length > 0 && res[0].Length > 100)
            {
                sb.AppendLine("");
            }

            foreach (var line in res)
            {
                if (line.Length == 0) sb.AppendLine(line);
                else
                    sb.AppendLine(minws.HasValue ? line.Substring(minws.Value) : line);
            }

            return sb.ToString();
        }

        private static string ParseStrToCode(string str, string path = null)
        {
            str = str.Replace("\r", string.Empty)
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\n", "<br/>");
            
            var res = "<pre class='lang-java'>" + str + "</pre>";
            
            if (path != null)
            {
                res = "<div class='codePath'>" + path + "</div>" + res;
            }

            res = "<div class='codeWrap'>" + res + "</div>";

            return res;
        }
    }
}