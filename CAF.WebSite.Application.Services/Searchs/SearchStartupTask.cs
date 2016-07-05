using CAF.Infrastructure.Core;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Searchs
{
    /// <summary>
    /// 系统启动初始化搜索引擎
    /// </summary>
    public class SearchStartupTask : IStartupTask
    {

        public void Execute()
        {
            //var _instance = EngineContext.Current.Resolve<ISearchProvider>();
            //_instance.Init();

            //_instance.RemoveAll();

            ////List<IndexItem> pages = new List<IndexItem>();

            ////foreach (IndexItem page in pages)
            ////{
            ////    this._instance.IndexPage(page);
            ////}

            //_instance.IndexingFinished();

        }

        public int Order
        {
            get { return 100; }
        }
    }
}