using System.Web.Optimization;

namespace CAF.WebSite.Application.WebUI.Mvc.Bundles
{
    /// <summary>
    /// <remarks>codehint: caf-add</remarks>
    /// </summary>
    public interface IBundlePublisher
    {
        void RegisterBundles(BundleCollection bundles);
    }
}
