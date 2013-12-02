namespace OpenAA.Tests
{
    using NUnit.Framework;
    using System;
    using System.Net;
    using System.Net.Http;
    using OpenAA.Net;

    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestCase()
        {
            var url = "http://www.ugtop.com/spill.shtml";
            var agent = new OpenAA.Net.HttpAgent();
            var task = agent.GetStringWithAutoDetectEncodingAsync(url);
            task.Wait();

            Assert.IsNotEmpty(task.Result);
        }
    }
}

