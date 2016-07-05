 
namespace CAF.Infrastructure.Search.Analyzers {
    using Configuration;
    using Lucene.Net.Analysis;
    /// <summary>
    /// 盘古分词
    /// </summary>
    public class PanGuAnalyzer : IAnalyzer {
        public PanGuAnalyzer() {
            Analyzer = new Lucene.Net.Analysis.PanGu.PanGuAnalyzer();
        }

        public Analyzer Analyzer { get; private set; }
    }
}