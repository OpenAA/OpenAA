﻿namespace OpenAA.Monazilla.Tests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using OpenAA.Monazilla;
    using OpenAA.Extensions.IEnumerable;

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

            var cate = t1.Result.First(x => x.Name == "ネット関係");
            Console.WriteLine(cate);

            var board = cate.Boards.First(x => x.Name.Contains("宣伝掲示板"));
            Console.WriteLine(board);

            var t2 = mona.GetSubject(board);
            t2.Wait();
            var threads = t2.Result;
            foreach (var thread in threads)
            {
                Console.WriteLine(thread);
            }
        }

        [Test]
        public void CreateResponse()
        {
            var mona = new MonaAgent();
            var t1 = mona.GetCategories();
            t1.Wait();

            var cate = t1.Result.First(x => x.Name == "ネット関係");
            Console.WriteLine(cate);

            var board = cate.Boards.First(x => x.Name.Contains("宣伝掲示板"));
            Console.WriteLine(board);

            var t2 = mona.GetSubject(board);
            t2.Wait();

            for (int i = 0; i < 10; i++)
            {
                var thread = t2.Result
                             //.Where(x => 10 < x.Nums)
                             //.OrderByDescending(x => x.CreateTime)
                    .Shuffle()
                    .First();
                Console.WriteLine(thread);

                var t3 = mona.CreateResponse(thread, "はげ", "hage", "はげちゃびん");
                t3.Wait();
            }
        }
    }
}

