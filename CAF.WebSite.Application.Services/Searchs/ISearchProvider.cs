using CAF.Infrastructure.Search.Core;
using CAF.WebSite.Application.Services.Searchs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Searchs
{
    public interface ISearchProvider
    {
        /// <summary>
        /// 添加索引
        /// </summary>
        /// <param name="item"></param>
        void AddToIndex(IndexItem item);
        /// <summary>
        /// 移除所有
        /// </summary>
        void RemoveAll();
        /// <summary>
        /// 移除索引
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="languageId"></param>
        void RemoveFromIndex(string itemId, int languageId);

        void RemoveFromIndex(Collection<string> itemIds, int languageId);

        void IndexingFinished();

        void Init();

        SearchResult Search(SearchQuery query);

        SearchResult FindSimular(IndexItem page, int resultOffset = 0, int resultSize = 10, bool matchCategory = true);

        SearchResult FindSimular(string itemId, int languageId, int resultOffset = 0, int resultSize = 10, bool matchCategory = true);

        void IndexPage(IndexItem indexItem);
    }
}
