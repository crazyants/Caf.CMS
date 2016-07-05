using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class WhiteSpaceFilter : Stream
    {
        private Stream _shrink;
        private Func<string, string> _filter;

        public WhiteSpaceFilter(Stream shrink, Func<string, string> filter)
        {
            _shrink = shrink;
            _filter = filter;
        }


        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return true; } }
        public override void Flush() { _shrink.Flush(); }
        public override long Length { get { return 0; } }
        public override long Position { get; set; }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _shrink.Read(buffer, offset, count);
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _shrink.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            _shrink.SetLength(value);
        }
        public override void Close()
        {
            _shrink.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // capture the data and convert to string
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            string s = Encoding.UTF8.GetString(buffer);

            // filter the string
            s = _filter(s);

            // write the data to stream
            byte[] outdata = Encoding.UTF8.GetBytes(s);
            _shrink.Write(outdata, 0, outdata.GetLength(0));
        }
    }
    /// <summary>
    /// 压缩 HTML
    /// </summary>
    /// <remarks>压缩 html 可以去除代码中无用的空格等，这样可提高网站的加载速度并节省带宽</remarks>
    public class WhitespaceFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;
            if (response.Filter == null) return;
            response.Filter = new WhiteSpaceFilter(response.Filter, s =>
            {
                ///正则会将<pre></pre>中的空格也进行替换，并指定编码为 UTF-8
                s = Regex.Replace(s, @"\s+(?=<)|\s+$|(?<=>)\s+", "");

                //single-line doctype must be preserved
                var firstEndBracketPosition = s.IndexOf(">");
                if (firstEndBracketPosition >= 0)
                {
                    s = s.Remove(firstEndBracketPosition, 1);
                    s = s.Insert(firstEndBracketPosition, ">");
                }
                return s;
            });
        }
    }
}