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
        [TestCase("../../../../TestData/not_a_valid_docx.docx")]
        //[TestCase("2")]
        public void DocReadWord_Test(string testCase)
        {
            var file = new FileInfo("../../../../TestData/word" + testCase + ".docx");
            var strActual = Doc.Read(file);
            var strExpected = "�����ĵ�" + testCase + "�����ĵ�";
            if (testCase.Contains("valid")) strExpected = "not_a_valid_docx";
            Assert.AreEqual(strExpected, strActual);
        }

        [TestCase("../../../../TestData/pp1.pptx")]
        [TestCase("../../../../TestData/not_a_valid_pptx.pptx")]
        public void DocReadPowerPoint_Test(string testCase)
        {
            var file = new FileInfo(testCase);
            var strActual = Doc.Read(file);
            var strExpected = "���Ĳ��� Helloworld ���� ";
            if (testCase.Contains("valid")) strExpected = "not_a_valid_pptx";
            Assert.AreEqual(strExpected, strActual);
        }

        [TestCase("../../../../TestData/txt.txt")]
        [TestCase("not_existed.txt")]
        public void DocReadTxt_Test(string testCase)
        {
            var file = new FileInfo(testCase);
            var strExpected = "Hello world!\r\n������磡";
            if (!file.Exists) strExpected = "not_existed";
            var strActual = Doc.Read(file);
            Assert.AreEqual(strExpected, strActual);
        }

        [TestCase("../../../../TestData/ss1.xlsx")]
        [TestCase("../../../../TestData/not_a_valid_xlsx.xlsx")]
        public void DocReadExcel_Test(string testCase)
        {
            var file = new FileInfo(testCase);
            var strActual = Doc.Read(file);
            var strExpected = "Sheet3 �� ���� ֵ TRUE Sheet2 Ҳ �� ���� ���� �ĵ� ��� �� ���� ";
            if (testCase.Contains("valid")) strExpected = "not_a_valid_xlsx";
            Assert.AreEqual(strExpected, strActual);
        }

        [Test]
        public void DocReadNone_Test()
        {
            const string strExpected = "test1 test";
            var strActual = Doc.Read(new FileInfo("test1.test"));
            Assert.AreEqual(strExpected, strActual);
        }
    }
}