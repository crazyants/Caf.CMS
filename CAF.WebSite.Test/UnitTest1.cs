using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAF.WebSite.Application.WebUI.Security;


namespace CAF.WebSite.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var  moduleJson = SSOAuthHelper.GetPermissions("18e48b8245e1a85d1d635ff28d561822");

        }
        [TestMethod]
        public void TestMethod12()
        {
            var moduleJson = SSOAuthHelper.GetCertificate("18e48b8245e1a85d1d635ff28d561822");

        }
    }
}
