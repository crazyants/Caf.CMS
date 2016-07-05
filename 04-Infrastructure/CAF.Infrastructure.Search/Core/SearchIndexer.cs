namespace CAF.Infrastructure.Search.Core
{
    using System;
    using System.IO;
    using Lucene.Net.Analysis;
    using Lucene.Net.Index;
    using Lucene.Net.Store;

    public class SearchIndexer : IDisposable
    {
        private readonly FSDirectory _directory;
        private readonly IndexWriter _indexWriter;
        private readonly Analyzer _analyzer;
        private bool _open;

        internal IndexWriter IndexWriter
        {
            get
            {
                return _indexWriter;
            }
        }

        public SearchIndexer(FSDirectory directory, Analyzer analyzer)
        {
            _directory = directory;
            _analyzer = analyzer;
            EnsureThatIndexIsUnlocked();
            bool isUpdate = IndexReader.IndexExists(_directory); //判断索引库是否存在
            
            //  创建向索引库写操作对象  IndexWriter(索引目录,指定使用分词进行切词,最大写入长度限制)
            //  补充:使用IndexWriter打开directory时会自动对索引库文件上锁
            _indexWriter = new IndexWriter(directory, _analyzer, !isUpdate, IndexWriter.MaxFieldLength.UNLIMITED);
            _open = true;
        }

        public void AddDocument(IndexDocument document)
        {
            _indexWriter.AddDocument(document.Document);
        }

        public void Close()
        {
            _analyzer.Close();
            _indexWriter.Dispose();
            _open = false;
        }

        private void EnsureThatIndexIsUnlocked()
        {
            bool isUpdate = IndexReader.IndexExists(_directory); //判断索引库是否存在
            if (isUpdate)
            {
                //  如果索引目录被锁定（比如索引过程中程序异常退出），则首先解锁
                //  Lucene.Net在写索引库之前会自动加锁，在close的时候会自动解锁
                //  不能多线程执行，只能处理意外被永远锁定的情况
                if (IndexWriter.IsLocked(_directory))
                {
                    IndexWriter.Unlock(_directory);//unlock:强制解锁，待优化
                }
                DirectoryInfo directoryInfo = _directory.Directory;
                var lockFilePath = Path.Combine(directoryInfo.FullName, "write.lock");

                if (File.Exists(lockFilePath))
                {
                    File.Delete(lockFilePath);
                }
            }
          
        }

        public void OptimizeIndex()
        {
            _indexWriter.Optimize();
            _indexWriter.Commit();
            Dispose();
        }

        public void RemoveAll()
        {
            _indexWriter.DeleteAll();
            Dispose();
        }

        public void RemoveDocument(string key)
        {
            var term = new Term("key", key);
            _indexWriter.DeleteDocuments(term);
            Dispose();
        }

        public void Dispose()
        {
            if (_open)
            {
                Close();
            }
        }

    }
}