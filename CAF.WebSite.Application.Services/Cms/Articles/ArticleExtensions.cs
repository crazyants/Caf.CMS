using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
    public static class ArticleExtensions
    {

        /// <summary>
        /// Get a default picture of a article 
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="pictureService">Picture service</param>
        /// <returns>Article picture</returns>
        public static Picture GetDefaultArticlePicture(this Article source, IPictureService pictureService)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (pictureService == null)
                throw new ArgumentNullException("pictureService");

            var picture = pictureService.GetPicturesByArticleId(source.Id, 1).FirstOrDefault();
            return picture;
        }


        public static bool ArticleTagExists(this Article article, int articleTagId)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            bool result = article.ArticleTags.ToList().Find(pt => pt.Id == articleTagId) != null;
            return result;
        }

        public static string GetArticleStatusLabel(this Article article, ILocalizationService localizationService)
        {
            if (article != null )
            {
                string key = "Admin.ContentManagement.Articles.ArticleStatus.{0}.Label".FormatWith(article.ArticleStatus.ToString());
                return localizationService.GetResource(key);
            }
            return "";
        }

        /// <summary>
        /// Finds a related article item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="articleId2">The second article identifier</param>
        /// <returns>Related article</returns>
        public static RelatedArticle FindRelatedArticle(this IList<RelatedArticle> source,
            int articleId1, int articleId2)
        {
            foreach (RelatedArticle relatedArticle in source)
                if (relatedArticle.ArticleId1 == articleId1 && relatedArticle.ArticleId2 == articleId2)
                    return relatedArticle;
            return null;
        }
    }
}
