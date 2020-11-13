using System;
using FileType;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class FileType
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_Null()
        {
            try
            {
                FileTypeMap.GetType(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.Pass();
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void Test_Dot_File()
        {
            var tc = FileTypeMap.GetType("txt");
            Assert.AreEqual(FileTypeMap.TypeCode.TypeText, tc);
        }
    }
}