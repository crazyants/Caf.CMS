using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Articles
{
    public class ArticlesSearchingEvent
    {
        public ArticlesSearchingEvent(ArticleSearchContext ctx)
        {
            SearchContext = ctx;
        }

        public ArticleSearchContext SearchContext { get; private set; }
    }
}
