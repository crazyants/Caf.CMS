using CAF.WebSite.Application.WebUI.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace CAF.WebSite.Application.WebUI
{
    public static class ExtendMvcHtml
    {
        /// <summary>
        /// 权限按钮
        /// </summary>
        /// <param name="helper">htmlhelper</param>
        /// <param name="id">控件Id</param>
        /// <param name="icon">控件icon图标class</param>
        /// <param name="text">控件的名称</param>
        /// <param name="perm">权限列表</param>
        /// <param name="keycode">操作码</param>
        /// <param name="hr">分割线</param>
        /// <returns>html</returns>
        public static MvcHtmlString ToolButton(this HtmlHelper helper, string id, string icon, string text, IList<PerBtnModel> perm, string keycode, bool hr)
        {
            if (perm.Where(a => a.ButtonCode == keycode).Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<a id=\"{0}\" style=\"float: left;\" class=\"l-btn l-btn-plain\">", id);
                sb.AppendFormat("<span class=\"l-btn-left\"><span class=\"l-btn-text {0}\" style=\"padding-left: 20px;\">", icon);
                sb.AppendFormat("{0}</span></span></a>", text);
                if (hr)
                {
                    sb.Append("<div class=\"datagrid-btn-separator\"></div>");
                }
                return new MvcHtmlString(sb.ToString());
            }
            else
            {
                return new MvcHtmlString("");
            }
        }
        /// <summary>
        /// 普通按钮
        /// </summary>
        /// <param name="helper">htmlhelper</param>
        /// <param name="id">控件Id</param>
        /// <param name="icon">控件icon图标class</param>
        /// <param name="text">控件的名称</param>
        /// <param name="hr">分割线</param>
        /// <returns>html</returns>
        public static MvcHtmlString ToolButton(this HtmlHelper helper, string id, string icon, string text, bool hr)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<a id=\"{0}\" style=\"float: left;\" class=\"l-btn l-btn-plain\">", id);
            sb.AppendFormat("<span class=\"l-btn-left\"><span class=\"l-btn-text {0}\" style=\"padding-left: 20px;\">", icon);
            sb.AppendFormat("{0}</span></span></a>", text);
            if (hr)
            {
                sb.Append("<div class=\"datagrid-btn-separator\"></div>");
            }
            return new MvcHtmlString(sb.ToString());

        }

        /// <summary>
        /// 普通按钮
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">控件的名称</param>
        /// <param name="buttonText">按钮Text</param>
        /// <param name="disabled">隐藏显示</param>
        /// <param name="htmlAttributes">属性</param>
        /// <returns></returns>
        private static MvcHtmlString Button(this HtmlHelper helper, string name, string buttonText, bool disabled, IList<KeyValuePair<string, object>> htmlAttributes)
        {
            HtmlGenericControl a = new HtmlGenericControl("input");
            a.ID = name;
            a.Attributes["name"] = name;
            a.Attributes["value"] = buttonText; a.Attributes["type"] = "button";
            if (disabled) a.Attributes["disabled"] = "disabled";
            if (htmlAttributes != null)
                foreach (KeyValuePair<string, object> attribute in htmlAttributes)
                {
                    a.Attributes[attribute.Key] = attribute.Value.ToString();
                }
            StringBuilder htmlBuilder = new StringBuilder();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(new StringWriter(htmlBuilder));
            return new MvcHtmlString(htmlBuilder.ToString());
        }
    }
}
