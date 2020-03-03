using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Index
{
    public class Index
    {
        private LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        private IndexWriter writer;

        private Index(string indexLocation)
        {
            var directory = FSDirectory.Open(indexLocation);
            var analyzer = new JieBaAnalyzer(TokenizerMode.Search);
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            writer = new IndexWriter(directory, indexConfig);
        }


    }
}