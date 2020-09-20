using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Database.Test
{
    public class Tests
    {
        private static readonly Random _rd = new Random();
        private Engine _index;

        [SetUp]
        public void Setup()
        {
            Directory.Delete("./index", true);
            _index = new Engine("./index");
        }

        [Test]
        public void Scheme_Add_Search_Test()
        {
            var doc = Engine.GenerateDocument("path", "content", "pinyin", "");
            _index.Add(doc);
            for (var i = 0; i < 100; i++)
            {
                var path = GenerateRandomString(10);
                _index.Add(
                    Engine.GenerateDocument(
                        path,
                        "Index Query: " + i + GenerateRandomString(100),
                        "pinyin " + GenerateRandomString(100),
                        "x"
                    )
                );
                if (i == 99) _index.Delete(path);
            }

            for (var i = 0; i < 100; i++)
                _index.Add(Engine.GenerateDocument(new Engine.Scheme
                        {
                            Content = "Index Query: " + i + GenerateRandomString(100),
                            Path = GenerateRandomString(10),
                            Pinyin = GenerateRandomString(10),
                            Tag = "y"
                        }
                    )
                );

            var resultIndex = _index.Search("index", "x");
            var resultPinyin = _index.Search("pinyin", "y");
            Assert.AreEqual(99, resultIndex.Count());
            Assert.AreEqual(0, resultPinyin.Count());

            _index.DeleteAll();
            resultIndex = _index.Search("index", "x");
            Assert.AreEqual(0, resultIndex.Count());

            _index.Close();
        }

        private static string GenerateRandomString(int length)
        {
            char[] constant =
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
                'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
                'u', 'v', 'w', 'x', 'y', 'z'
            };

            var checkCode = string.Empty;
            for (var i = 0; i < length; i++) checkCode += constant[_rd.Next(36)].ToString();

            return checkCode;
        }
    }
}