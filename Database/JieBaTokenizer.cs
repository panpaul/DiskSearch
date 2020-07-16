using System.Collections.Generic;
using System.IO;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Token = JiebaNet.Segmenter.Token;

namespace Database
{
    internal class JieBaTokenizer : Tokenizer
    {
        private readonly TokenizerMode _mode;
        private readonly JiebaSegmenter _segmenter;
        private readonly string _stopUrl = ConfigManager.StopWordsFile;

        private readonly List<string> _stopWords = new List<string>();
        private readonly List<Token> _wordList = new List<Token>();
        private string _inputText;

        private IEnumerator<Token> _iter;
        private IOffsetAttribute _offsetAtt;

        private ICharTermAttribute _termAtt;
        private ITypeAttribute _typeAtt;


        public JieBaTokenizer(TextReader input, TokenizerMode mode)
            : base(AttributeFactory.DEFAULT_ATTRIBUTE_FACTORY, input)
        {
            _segmenter = new JiebaSegmenter();
            _mode = mode;
            var rd = File.OpenText(_stopUrl);
            string s;
            while ((s = rd.ReadLine()) != null) _stopWords.Add(s);

            Init();
        }

        private void Init()
        {
            _termAtt = AddAttribute<ICharTermAttribute>();
            _offsetAtt = AddAttribute<IOffsetAttribute>();
            _typeAtt = AddAttribute<ITypeAttribute>();
        }

        public sealed override bool IncrementToken()
        {
            ClearAttributes();

            var word = Next();
            if (word != null)
            {
                var buffer = word.ToString();
                _termAtt.SetEmpty().Append(buffer);
                _offsetAtt.SetOffset(CorrectOffset(word.StartOffset), CorrectOffset(word.EndOffset));
                _typeAtt.Type = word.Type;
                return true;
            }

            End();
            Dispose();
            return false;
        }

        private Lucene.Net.Analysis.Token Next()
        {
            var res = _iter.MoveNext();
            if (!res) return null;

            var word = _iter.Current;
            if (word == null) return null;
            var token = new Lucene.Net.Analysis.Token(word.Word, word.StartIndex, word.EndIndex);
            return token;

        }

        public override void Reset()
        {
            base.Reset();

            _inputText = m_input.ReadToEnd();
            RemoveStopWords(_segmenter.Tokenize(_inputText, _mode));

            _iter = _wordList.GetEnumerator();
        }

        private void RemoveStopWords(IEnumerable<Token> words)
        {
            _wordList.Clear();

            foreach (var x in words)
                if (_stopWords.IndexOf(x.Word) == -1)
                    _wordList.Add(x);
        }
    }
}