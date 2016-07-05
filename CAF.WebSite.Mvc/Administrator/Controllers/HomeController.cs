
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Mvc.Admin.Models.Home;
using CAF.WebSite.Mvc.Admin.Models.Users;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class HomeController : AdminControllerBase
    {
        private readonly IVisitRecordService _visitRecordService;
        public HomeController(IVisitRecordService visitRecordService)
        {
            this._visitRecordService = visitRecordService;
        }
        // GET: Passport
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult BasicColumn()
        {
            Highcharts chart = new Highcharts("chart")
                .InitChart(new Chart { Type = ChartTypes.Column })
                .SetTitle(new Title { Text = "Monthly Average Rainfall" })
                .SetSubtitle(new Subtitle { Text = "Source: WorldClimate.com" })
                .SetXAxis(new XAxis { Categories = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" } })
                .SetYAxis(new YAxis
                {
                    Min = 0,
                    Title = new YAxisTitle { Text = "Rainfall (mm)" }
                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Left,
                    VerticalAlign = VerticalAligns.Top,
                    X = 100,
                    Y = 70,
                    Floating = true,
                    BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                    Shadow = true
                })
                .SetTooltip(new Tooltip { Formatter = @"function() { return ''+ this.x +': '+ this.y +' mm'; }" })
                .SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        PointPadding = 0.2,
                        BorderWidth = 0
                    }
                })
                .SetSeries(new[]
                {
                    new Series { Name = "Tokyo", Data = new Data(new object[] { 49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4 }) },
                    new Series { Name = "London", Data = new Data(new object[] { 48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2 }) },
                    new Series { Name = "New York", Data = new Data(new object[] { 83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3 }) },
                    new Series { Name = "Berlin", Data = new Data(new object[] { 42.4, 33.2, 34.5, 39.7, 52.6, 75.5, 57.4, 60.4, 47.6, 39.1, 46.8, 51.1 }) }
                });

            return View(chart);
        }

        [ChildActionOnly]
        public ActionResult ReportVisit()
        {
            var model = this._visitRecordService.GetVisitRecordStatistics<ReportVisitModel>(DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd"));
            return PartialView(model);
        }

    }
}