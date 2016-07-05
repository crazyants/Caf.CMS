using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Catalog
{
    public class ProductsSearchingEvent
    {
        public ProductsSearchingEvent(ProductSearchContext ctx)
        {
            SearchContext = ctx;
        }

        public ProductSearchContext SearchContext { get; private set; }
    }
}
