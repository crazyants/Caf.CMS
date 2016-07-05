using CAF.Infrastructure.Search.Core;
using System;
using System.Collections.ObjectModel;

namespace CAF.WebSite.Application.Services.Searchs
{

    public class NullSearchProvider : SearchProviderBase {
        public override void AddToIndex(IndexItem item) {
        }

        public override void RemoveAll() {
        }

        public override void RemoveFromIndex(string itemId, int languageId)
        {
        }

        public override void RemoveFromIndex(Collection<string> itemId, int languageId)
        {
        }

        public override void IndexingFinished() {
        }

        public override void Init() {
        }

        public override SearchResult Search(SearchQuery query)
        {
            throw new Exception("Can't search using NullSearchProvider, change to a valid provider in config.");
        }

        public override SearchResult FindSimular(string itemId, int languageId, int resultOffset = 0, int resultSize = 10, bool matchCategory = true)
        {
            throw new Exception("Can't search using NullSearchProvider, change to a valid provider in config.");
        }
    }
}
