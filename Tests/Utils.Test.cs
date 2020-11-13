using NUnit.Framework;
using Utils;

namespace Tests
{
    [TestFixture]
    public class Utils
    {
        [SetUp]
        public void Setup()
        {
            var path = "config.json";
            if (TestContext.CurrentContext.WorkDirectory.Contains("net5.0"))
                // test located in \bin\Debug\net5.0
                path = "../../../../TestData/config/" + path;
            path = "./TestData/config/" + path;
            _config = new Config(path);
        }

        private Config _config;

        [Test]
        public void Utils_Read()
        {
            _config.Read();
            Assert.AreEqual(@"TEST_LO:C",_config.SearchPath);
        }

        [Test]
        public void Utils_Save()
        {
            _config.SearchPath = "New";
            _config.Save();
            _config.Read();
            Assert.AreEqual("New",_config.SearchPath);

            _config.SearchPath = @"TEST_LO:C";
            _config.Save();
            _config.Read();
            Assert.AreEqual(@"TEST_LO:C", _config.SearchPath);
        }
    }
}