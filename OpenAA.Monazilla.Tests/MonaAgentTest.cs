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
        [Test]
        public void GetBbsMenuHtml()
        {
            var agent = new MonaAgent();
            var t1 = agent.GetBbsMenuHtml();
            t1.Wait();
            Console.WriteLine(t1.Result);
        }

        [Test()]
        public void GetBoards()
        {
            var agent = new MonaAgent();
            var t1 = agent.GetBoards();
            t1.Wait();

            foreach (var board in t1.Result)
            {
                Console.WriteLine(board);
            }
        }

        [Test]
        public void GetBoard1()
        {
            var agent = new MonaAgent();
            var task1 = agent.GetBoard("http://engawa.2ch.net/poverty/");
            task1.Wait();
            var board = task1.Result;
            Console.WriteLine(board);
            Assert.IsNotNull(board);
        }

        [Test]
        public void GetBoard2()
        {
            var agent = new MonaAgent();
            var task1 = agent.GetBoard("http://engawa.2ch.net/test/read.cgi/poverty/1386076505/");
            task1.Wait();
            var board = task1.Result;
            Assert.IsNotNull(board);
        }

        [Test]
        public void GetSubject()
        {
            var agent = new MonaAgent();
            var t1 = agent.GetBoards();
            t1.Wait();
            var board = t1.Result.First(x => x.Id == "ad");
            Console.WriteLine(board);

            var t2a = agent.GetSubject(board);
            t2a.Wait();

            Assert.IsNotEmpty(t2a.Result);
        }

        [Test]
        public void GetThreads()
        {
            var agent = new MonaAgent();
            var t1 = agent.GetBoards();
            t1.Wait();
            var board = t1.Result.First(x => x.Id == "ad");
            Console.WriteLine(board);

            var task2 = agent.GetThreadsAsync(board);
            task2.Wait();
            var threads = task2.Result;
            foreach (var thread in threads)
            {
                Console.WriteLine(thread);
            }
        }

        [Test]
        public void CreateResponse()
        {
            var agent = new MonaAgent();
            var t1 = agent.GetBoards();
            t1.Wait();
            var board = t1.Result.First(x => x.Id == "ad");
            Console.WriteLine(board);

            var task2 = agent.GetThreadsAsync(board);
            task2.Wait();
            var thread = task2.Result
                .OrderByDescending(x => x.Trend)
                .First(x => 10 < x.Nums && x.Nums < 1000);

            var task3 = agent.CreateResponse(thread, "はげ", "hage", "はげちゃびん");
            task3.Wait();
        }

        [Test]
        public void GetDat()
        {
            var agent = new MonaAgent();
            var t1 = agent.GetBoards();
            t1.Wait();
            var board = t1.Result.First(x => x.Id == "ad");

            var task2 = agent.GetThreadsAsync(board);
            task2.Wait();
            var thread = task2.Result
                .OrderByDescending(x => x.Trend)
                .First(x => 10 < x.Nums && x.Nums < 1000);
            Console.WriteLine(thread);

            var task3 = agent.GetDat(thread);
            task3.Wait();

            var task4 = agent.CreateResponse(thread, "はげ", "hage", "はげちゃびん");
            task4.Wait();

            var task5 = agent.GetDat(thread);
            task5.Wait();
        }

        [Test]
        public void GetResponses()
        {
            var agent = new MonaAgent();
            var t1 = agent.GetBoards();
            t1.Wait();
            var board = t1.Result.First(x => x.Id == "ad");

            var task2 = agent.GetThreadsAsync(board);
            task2.Wait();
            var thread = task2.Result
                .OrderByDescending(x => x.Trend)
                .First(x => 10 < x.Nums && x.Nums < 1000);

            var task3 = agent.GetResponses(thread);
            task3.Wait();
            var res = task3.Result;
            foreach (var one in res)
            {
                Console.WriteLine(one);
            }
        }
    }

    public static class TaskExtensions
    {
        public static System.Threading.Tasks.Task<T> Wait2<T>(this System.Threading.Tasks.Task<T> task)
        {
            task.Wait();
            return task;
        }
    }
}

