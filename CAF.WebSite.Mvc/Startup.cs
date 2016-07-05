#region usings
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using WebGrease.Extensions;
using Microsoft.AspNet.SignalR;
 

#endregion

[assembly: OwinStartup(typeof(CAF.WebSite.Mvc.Startup))]
namespace CAF.WebSite.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableJavaScriptProxies = true;
            //hubConfiguration.EnableDetailedErrors = true;
            //hubConfiguration.EnableJavaScriptProxies = true;
            app.MapSignalR("/signalr", hubConfiguration);
        }

    
    }
 
}
