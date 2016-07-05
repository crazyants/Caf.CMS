using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Mvc.Admin.Models.Home
{
    public class ReportVisitModel
    {
        public string VisitDate { get; set; }
        public int UV { get; set; }
        public int PV { get; set; }
        public int BaiduIP { get; set; }
        public string YiDiaoLv { get; set; }
    }
}