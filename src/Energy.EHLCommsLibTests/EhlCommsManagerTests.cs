using Energy.EHLCommsLib;
using NUnit.Framework;

namespace Energy.EHLCommsLibTests
{
    [TestFixture]
    public class EhlCommsManagerTests
    {
        private EhlCommsAggregator EhlCommsAggregator { get; set; }

        [SetUp]
        public void SetUp()
        {
            //EhlCommsAggregator = new EhlCommsAggregator();
        }

        [Test]
        public void HelloWorldTest()
        {
            Assert.AreEqual(EhlCommsAggregator.Test(), "Hello World", "As a raw version of lib and test, we expect Hello world string as a first result");
        }
    }
}
