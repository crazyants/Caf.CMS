using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Searchs
{
    public class ArticleEvent
    {
        public ArticleEvent(Article article, string url)
        {
            this.Article = article;
            this.Url = url;
        }

        public Article Article { get; set; }
        public string Url { get; set; }
    }
}
