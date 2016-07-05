using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Articles;
using System;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Models.ArticleCatalog     
{
    public partial class PopularArticlesTagsModel : ModelBase
    {
        public PopularArticlesTagsModel()
        {
            Tags = new List<ArticleTagModel>();
        }

        #region Utilities

        protected virtual int GetFontSize(double weight, double mean, double stdDev)
        {
            double factor = (weight - mean);

            if (factor != 0 && stdDev != 0) factor /= stdDev;

            return (factor > 2) ? 150 :
                (factor > 1) ? 120 :
                (factor > 0.5) ? 100 :
                (factor > -0.5) ? 90 :
                (factor > -1) ? 85 :
                (factor > -2) ? 80 :
                75;
        }

        protected virtual double Mean(IEnumerable<double> values)
        {
            double sum = 0;
            int count = 0;

            foreach (double d in values)
            {
                sum += d;
                count++;
            }

            return sum / count;
        }

        protected virtual double StdDev(IEnumerable<double> values, out double mean)
        {
            mean = Mean(values);
            double sumOfDiffSquares = 0;
            int count = 0;

            foreach (double d in values)
            {
                double diff = (d - mean);
                sumOfDiffSquares += diff * diff;
                count++;
            }

            return Math.Sqrt(sumOfDiffSquares / count);
        }

        #endregion

        #region Methods

        public virtual int GetFontSize(ArticleTagModel productTag)
        {
            double mean = 0;
            var itemWeights = new List<double>();
            foreach (var tag in Tags)
                itemWeights.Add(tag.ArticleCount);
            double stdDev = StdDev(itemWeights, out mean);

            return GetFontSize(productTag.ArticleCount, mean, stdDev);
        }

        #endregion

        #region Properties

        public int TotalTags { get; set; }

        public IList<ArticleTagModel> Tags { get; set; }

        #endregion
    }
}