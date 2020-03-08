﻿using System.Collections.Generic;
using JiebaNet.Segmenter;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Database
{
    public class Engine
    {
        private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        private readonly JieBaAnalyzer _analyzer = new JieBaAnalyzer(TokenizerMode.Search);
        private readonly FSDirectory _directory;
        private readonly IndexWriter _writer;

        public Engine(string indexLocation)
        {
            _directory = FSDirectory.Open(indexLocation);
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, _analyzer);
            _writer = new IndexWriter(_directory, indexConfig);
        }

        public static Document GenerateDocument(string path, string content)
        {
            var doc = new Document
            {
                new StringField("Path", path, Field.Store.YES),
                new TextField("Content", content, Field.Store.YES)
            };
            return doc;
        }

        public static Document GenerateDocument(Scheme s)
        {
            var doc = new Document
            {
                new StringField("Path", s.Path, Field.Store.YES),
                new TextField("Content", s.Content, Field.Store.YES)
            };
            return doc;
        }

        public void Add(Document doc)
        {
            _writer.UpdateDocument(new Term("Path", doc.Get("Path")), doc);
        }

        public void Flush()
        {
            _writer.Flush(true, true);
        }

        public IEnumerable<Scheme> Search(string word)
        {
            var queryPhrase = new QueryParser(AppLuceneVersion, "Content", _analyzer);

            var query = queryPhrase.Parse(word);

            var searcher = new IndexSearcher(_writer.GetReader(true));
            var hits = searcher.Search(query, 50).ScoreDocs;

            var results = new Scheme[hits.Length];

            var i = 0;
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                var result = new Scheme
                {
                    Path = foundDoc.Get("Path"),
                    Content = foundDoc.Get("Content")
                };
                results[i] = result;
                i++;
            }

            return results;
        }

        public void Close()
        {
            Flush();
            try
            {
                _writer.Dispose();
            }
            finally
            {
                if (IndexWriter.IsLocked(_directory)) IndexWriter.Unlock(_directory);
            }
        }

        public struct Scheme
        {
            public string Path;
            public string Content;
        }
    }
}