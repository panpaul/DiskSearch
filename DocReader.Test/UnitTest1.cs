using System.IO;
using NUnit.Framework;

namespace DocReader.Test
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("1")]
        //[TestCase("2")]
        public void DocReadWord_Test(string testCase)
        {
            var file = new FileInfo("../../../../TestData/word" + testCase + ".docx");
            var strActual = Doc.Read(file);
            var strExpected = "测试文档" + testCase + "测试文档";
            Assert.AreEqual(strExpected, strActual);
        }

        [Test]
        public void DocReadTxt_Test()
        {
            var file = new FileInfo("../../../../TestData/txt.txt");
            const string strExpected = "Hello world!\r\n你好世界！";
            var strActual = Doc.Read(file);
            Assert.AreEqual(strExpected, strActual);
        }

        [Test]
        public void DocReadNone_Test()
        {
            const string strExpected = "test1";
            var strActual = Doc.Read(new FileInfo("test1.test"));
            Assert.AreEqual(strExpected,strActual);
        }
    }
}