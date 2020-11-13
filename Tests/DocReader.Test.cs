using System.IO;
using DocReader;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DocReader
    {
        [SetUp]
        public void Setup()
        {
        }

        private static string Convert(string file)
        {
            if (TestContext.CurrentContext.WorkDirectory.Contains("net5.0"))
                // test located in \bin\Debug\net5.0
                return "../../../../TestData/" + file;
            return "./TestData/" + file;
        }

        [TestCase("word1.docx")]
        [TestCase("word2.docx")]
        [TestCase("not_a_valid_docx.docx")]
        public void DocReadWord_Test(string testCase)
        {
            var file = new FileInfo(Convert(testCase));

            var strActual = Doc.Read(file).Get("Content");
            var strExpected = "测试文档";
            if (testCase.Contains("valid")) strExpected = "not a valid";
            Assert.IsTrue(strActual.Contains(strExpected));

            strActual = Doc.GetPinyin(strActual);
            strExpected = "CE SHI WEN DANG CSWD";
            if (testCase.Contains("valid")) strExpected = " ";
            Assert.IsTrue(strActual.Contains(strExpected));
        }

        [TestCase("pp1.pptx")]
        [TestCase("not_a_valid_pptx.pptx")]
        public void DocReadPowerPoint_Test(string testCase)
        {
            var file = new FileInfo(Convert(testCase));

            var strActual = Doc.Read(file).Get("Content");
            var strExpected = "中文测试 Helloworld 数据";
            if (testCase.Contains("valid")) strExpected = "not a valid";
            Assert.IsTrue(strActual.Contains(strExpected));

            strActual = Doc.GetPinyin(strActual);
            strExpected = " ZHONG WEN CE SHI SHU JU ZWCS SJ ";
            if (testCase.Contains("valid")) strExpected = " ";
            Assert.IsTrue(strActual.Contains(strExpected));
        }

        [TestCase("txt.txt")]
        [TestCase("not_existed.txt")]
        public void DocReadTxt_Test(string testCase)
        {
            var file = new FileInfo(Convert(testCase));

            var strExpected = "Hello world! 你好世界！";
            if (!file.Exists) strExpected = "not";
            var strActual = Doc.Read(file).Get("Content");
            Assert.IsTrue(strActual.Contains(strExpected));

            strActual = Doc.GetPinyin(strActual);
            strExpected = " NI HAO SHI JIE NHSJ ";
            if (testCase.Contains("not")) strExpected = " ";
            Assert.IsTrue(strActual.Contains(strExpected));
        }

        [TestCase("ss1.xlsx")]
        [TestCase("not_a_valid_xlsx.xlsx")]
        public void DocReadExcel_Test(string testCase)
        {
            var file = new FileInfo(Convert(testCase));

            var strActual = Doc.Read(file).Get("Content");
            var strExpected =
                "Sheet3 有 布尔 值 TRUE Sheet2 也 有 数据 测试 文档 这儿 有 数据";
            if (testCase.Contains("valid")) strExpected = "not a valid xlsx";
            Assert.IsTrue(strActual.Contains(strExpected));

            strActual = Doc.GetPinyin(strActual);
            strExpected =
                " YOU BU ER ZHI YE YOU SHU JU CE SHI WEN DANG ZHEI ER YOU SHU JU Y BE Z Y Y SJ CS WD ZE Y SJ ";
            if (testCase.Contains("valid")) strExpected = " ";
            Assert.IsTrue(strActual.Contains(strExpected));
        }

        [TestCase("pdf.pdf")]
        [TestCase("pdf_invalid.pdf")]
        public void DocReadPdf_Test(string testCase)
        {
            var file = new FileInfo(Convert(testCase));

            var strActual = Doc.Read(file).Get("Content");
            var strExpected = "通知";

            if (testCase.Contains("invalid")) strExpected = "invalid";

            Assert.IsTrue(strActual.Contains(strExpected));
        }

        [TestCase("p_i_z_z_a.jpg")]
        [TestCase("pic_invalid.jpg")]
        public void DocReadImage_Test(string testCase)
        {
            var file = new FileInfo(Convert(testCase));

            var strActual = Doc.Read(file).Get("Content");
            var strExpected = "food";

            if (testCase.Contains("invalid")) strExpected = "Broken";

            Assert.IsTrue(strActual.Contains(strExpected));
        }

        [Test]
        public void DocReadNone_Test()
        {
            var strActual = Doc.Read(new FileInfo("test1.test")).Get("Content");
            const string strExpected = "test1.test";

            Assert.IsTrue(strActual.Contains(strExpected));
        }
    }
}