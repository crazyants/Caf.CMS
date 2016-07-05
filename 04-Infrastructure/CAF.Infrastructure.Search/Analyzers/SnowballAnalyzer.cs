
namespace CAF.Infrastructure.Search.Analyzers
{
    using Configuration;
    using Lucene.Net.Analysis;
    /// <summary>
    /// 经典分词用具 主要支持欧洲语言 
    /// </summary>
    public class SnowballAnalyzer : IAnalyzer
    {
        private readonly SearchSettings _settings;
        public SnowballAnalyzer(SearchSettings settings)
        {
            this._settings = settings;
            Analyzer = new Lucene.Net.Analysis.Snowball.SnowballAnalyzer(this._settings.LuceneVersion, this._settings.Language, StopWords.DefaultEnglish);
        }

        public Analyzer Analyzer { get; private set; }
    }
}