namespace OpenAA.Monazilla.Tests
{
    using NUnit.Framework;
    using System;
    using OpenAA.Monazilla;

    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestCase()
        {
            var mona = new MonaAgent();
            var task = mona.GetCategories();
            task.Wait();

            foreach (var cate in task.Result)
            {
                Console.WriteLine(cate);
            }

            NUnit.Framework.Assert.IsNotEmpty(task.Result);
        }
    }
}

