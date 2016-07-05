 
namespace CAF.Infrastructure.Search.Analyzers {
    using Lucene.Net.Analysis;

    public interface IAnalyzer {
        Analyzer Analyzer { get; }
    }
}
