using System;
using System.Collections.Generic;
using CAF.WebSite.Application.WebUI.Mvc;

namespace CAF.WebSite.Mvc.Models.Articles
{
    public partial class ArticlePostTagListModel : ModelBase
    {
        public ArticlePostTagListModel()
        {
            Tags = new List<ArticlePostTagModel>();
        }

        public int GetFontSize(ArticlePostTagModel blogPostTag)
        {
            double mean = 0;
            var itemWeights = new List<double>();
            foreach (var tag in Tags)
                itemWeights.Add(tag.ArticlePostCount);
            double stdDev = StdDev(itemWeights, out mean);

            return GetFontSize(blogPostTag.ArticlePostCount, mean, stdDev);
        }

        protected int GetFontSize(double weight, double mean, double stdDev)
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

        protected double Mean(IEnumerable<double> values)
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

        protected double StdDev(IEnumerable<double> values, out double mean)
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


        public IList<ArticlePostTagModel> Tags { get; set; }
    }
}