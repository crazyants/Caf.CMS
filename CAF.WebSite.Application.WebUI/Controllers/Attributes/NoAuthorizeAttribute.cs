using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.WebUI.Controllers.Attributes
{
    /// <summary>
    /// 表示当前不需要验证权限
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NoAuthorizeAttribute : Attribute
    {
    }
}
