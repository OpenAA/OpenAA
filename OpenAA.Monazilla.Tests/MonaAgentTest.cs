namespace OpenAA.Monazilla.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using OpenAA.Monazilla;

    [TestFixture()]
    public class MonaAgentTest
    {
        [Test()]
        public void GetCategories()
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

        [Test]
        public void GetSubject()
        {
            var mona = new MonaAgent();
            var t1 = mona.GetCategories();
            t1.Wait();

            var cate = t1.Result.First(x => x.Name == "雑談系２");
            Console.WriteLine(cate);

            var board = cate.Boards.First(x => x.Name.Contains("嫌儲"));
            Console.WriteLine(board);

            var t2 = mona.GetSubject(board);
            t2.Wait();
            var threads = t2.Result;
            foreach (var thread in threads)
            {
                Console.WriteLine(thread);
            }
        }
    }
}

