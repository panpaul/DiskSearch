using System;
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
            _index = new Engine("./index");
        }

        [Test]
        public void Scheme_Add_Search_Test()
        {
            var doc = Engine.GenerateDocument("path", "content");
            _index.Add(doc);
            for (var i = 0; i < 100; i++)
                _index.Add(Engine.GenerateDocument(GenerateRandomString(10),
                    "Index Query: " + i + GenerateRandomString(100)));

            for (var i = 0; i < 100; i++)
                _index.Add(Engine.GenerateDocument(new Engine.Scheme
                    {
                        Content = "Index Query: " + i + GenerateRandomString(100),
                        Path = GenerateRandomString(10)
                    }
                ));

            _index.Flush();

            var result = _index.Search("index");

            _index.Close();
            Assert.AreEqual(50, result.Count());
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