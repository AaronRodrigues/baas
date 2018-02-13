using Energy.EHLCommsLib;
using NUnit.Framework;

namespace Energy.EHLCommsLibTests
{
    [TestFixture]
    public class EhlCommsManagerTests
    {
        private EHLCommsManager ehlCommsManager { get; set; }

        [SetUp]
        public void SetUp()
        {
            ehlCommsManager = new EHLCommsManager();
        }

        [Test]
        public void HelloWorldTest()
        {
            Assert.AreEqual(ehlCommsManager.Test(), "Hello World", "As a raw version of lib and test, we expect Hello world string as a first result");
        }
    }
}
