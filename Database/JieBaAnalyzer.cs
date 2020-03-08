using System.IO;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Util;

namespace Database
{
    internal class JieBaAnalyzer : Analyzer
    {
        private readonly TokenizerMode _mode;

        public JieBaAnalyzer(TokenizerMode Mode)
        {
            _mode = Mode;
        }

        protected override TokenStreamComponents CreateComponents(string filedName, TextReader reader)
        {
            var tokenizer = new JieBaTokenizer(reader, _mode);

            var tokenStream = (TokenStream) new LowerCaseFilter(LuceneVersion.LUCENE_48, tokenizer);

            tokenStream.AddAttribute<ICharTermAttribute>();
            tokenStream.AddAttribute<IOffsetAttribute>();

            return new TokenStreamComponents(tokenizer, tokenStream);
        }
    }
}