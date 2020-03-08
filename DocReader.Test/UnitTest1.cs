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
        public void DocReadExcel_Test()
        {
            var file = new FileInfo("../../../../TestData/ss1.xlsx");
            var strActual = Doc.Read(file);
            var strExpected = "Sheet3 有 布尔 值 TRUE Sheet2 也 有 数据 测试 文档 这儿 有 数据 ";
            Assert.AreEqual(strExpected, strActual);
        }

        [Test]
        public void DocReadNone_Test()
        {
            const string strExpected = "test1";
            var strActual = Doc.Read(new FileInfo("test1.test"));
            Assert.AreEqual(strExpected, strActual);
        }

        [Test]
        public void DocReadPowerPoint_Test()
        {
            var file = new FileInfo("../../../../TestData/pp1.pptx");
            var strActual = Doc.Read(file);
            var strExpected = "中文测试 Helloworld 数据 ";
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
    }
}