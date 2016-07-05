/** 
* SimpleAnalyzer  这个分词是一段一段话进行分 
* StandardAnalyzer 标准分词拿来分中文和ChineseAnalyzer一样的效果 
☆PerFieldAnalyzerWrapper  这个很有意思，可以封装很多分词方式，还可以于先设置field用那个分词分！牛 
* CJKAnalyzer  这个分词方式是正向退一分词(二分法分词)，同一个字会和它的左边和右边组合成一个次，每个人出现两次，除了首字和末字 
* ChineseAnalyzer  这个是专业的中文分词器，一个一个字分 
* BrazilianAnalyzer 巴西语言分词 
* CzechAnalyzer 捷克语言分词 
* DutchAnalyzer 荷兰语言分词 
* FrenchAnalyzer 法国语言分词 
* GermanAnalyzer 德国语言分词 
* GreekAnalyzer 希腊语言分词 
* RussianAnalyzer 俄罗斯语言分词 
* ThaiAnalyzer 泰国语言分词 
* KeywordAnalyzer "Tokenizes" the entire stream as a single token. This is useful for data like zip codes, ids, and some product names. 
* PatternAnalyzer api讲这个分词方式很快，它是放在内存里面的 
* SnowballAnalyzer 经典分词用具 主要支持欧洲语言 
* StopAnalyzer 被忽略的词的分词器 
* WhitespaceAnalyzer 空格分词 
* */
namespace CAF.Infrastructure.Search.Analyzers
{
    using Configuration;
    using Lucene.Net.Analysis;
    /// <summary>
    /// 标准分词
    /// </summary>
    public class StandardAnalyzer : IAnalyzer
    {
        private readonly SearchSettings _settings;
        public StandardAnalyzer(SearchSettings settings)
        {
            this._settings = settings;
            Analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(this._settings.LuceneVersion);
        }

        public Analyzer Analyzer { get; private set; }
    }
}