 
namespace CAF.Infrastructure.Search.Analyzers
{
    using Configuration;
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.CWSharp;
    using System.IO;
    using Yamool.CWSharp;
    /// <summary>
    /// 中文分词组件 CWSharp
    /// </summary>
    public class CWSharpAnalyzer : IAnalyzer
    {
        private readonly SearchSettings _settings;
        public CWSharpAnalyzer(SearchSettings settings)
        {
            this._settings = settings;
            //初始化CWSharp
            var tokenizer = new StandardTokenizer(Path.Combine(this._settings.DataStorePath, "cwsharp.dawg"));
            Analyzer = new Lucene.Net.Analysis.CWSharp.CwsAnalyzer(tokenizer);
        }

        public Analyzer Analyzer { get; private set; }
    }
}