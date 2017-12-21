using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueWrapper;

namespace Tests
{
	[TestClass]
	public class QueueTests
	{
		private const int TestQueueSize = 1000;
		private static readonly TimeSpan DefaultWaitTime = new TimeSpan(0, 0, 0, 0, 50);

		/// <summary>
		/// Checks basic FIFO functionality
		/// </summary>
		[TestMethod]
		public void BasicQueueTest()
		{
			const string TestData1 = "TEST DATA 1";
			const string TestData2 = "TEST DATA 2";

			var queue = CreateQueue<string>();
			queue.Push(TestData1);
			queue.Push(TestData2);
			queue.Push("GARBAGE DATA");

			Assert.AreEqual(TestData1, queue.Pop());
			Assert.AreEqual(TestData2, queue.Pop());
		}

		/// <summary>
		/// Pop() must wait for data
		/// </summary>
		[TestMethod]
		public void TestPopWaits()
		{
			var expected = "TEST DATA";

			var queue = CreateQueue<string>();
			new Thread(() => {
				Thread.Sleep(DefaultWaitTime);
				queue.Push(expected);
			}).Start();

			Assert.AreEqual(expected, queue.Pop());
		}

		/// <summary>
		/// Simultaneous queue operations in parallel threads
		/// </summary>
		[TestMethod]
		public void TestMultithreading()
		{
			var queue = CreateQueue<int>();

			var thread1 = new Thread(() => {
				PushNumbers(1, TestQueueSize * 2, queue);
			});
			var thread2 = new Thread(() => {
				PushNumbers(1, TestQueueSize, queue);
			});
			var thread3 = new Thread(() => {
				PopNumbers(TestQueueSize, queue);
			});
			var thread4 = new Thread(() => {
				PopNumbers(TestQueueSize, queue);
			});
			thread1.Start();
			thread2.Start();
			thread3.Start();
			thread4.Start();

			while (thread1.IsAlive || thread2.IsAlive || thread3.IsAlive || thread4.IsAlive)
			{
				Thread.Sleep(DefaultWaitTime);
			}
			Assert.AreEqual(TestQueueSize, ((MultithreadedQueue<int>)queue).Count());
		}

		/// <summary>
		/// Simultaneous push/pop operations at the queue size of 0
		/// </summary>
		[TestMethod]
		[Timeout(500)]
		public void TestEdgeCase()
		{
			var queue = CreateQueue<int>();

			var thread1 = new Thread(() => {
				PopNumbers(TestQueueSize, queue);
			});
			var thread2 = new Thread(() => {
				PushNumbers(1, TestQueueSize + 1, queue);
			});

			thread1.Start();
			thread2.Start();

			while (thread1.IsAlive || thread2.IsAlive)
			{
				Thread.Sleep(DefaultWaitTime);
			}
			Assert.AreEqual(1, ((MultithreadedQueue<int>)queue).Count());
		}

		[TestMethod]
		[Timeout(500)]
		public void TwoPopsTest()
		{
			var queue = CreateQueue<int>();
			var result1 = 0;
			var result2 = 0;
			var thread1 = new Thread(() => {
				result1 = queue.Pop();
			});
			var thread2 = new Thread(() => {
				result2 = queue.Pop();
			});
			thread2.Start();
			Thread.Sleep(DefaultWaitTime);
			thread1.Start();

			queue.Push(44);
			while (thread2.IsAlive)
			{
				Thread.Sleep(DefaultWaitTime);
			}
			Assert.AreEqual(44, result2);

			queue.Push(33);
			while (thread1.IsAlive)
			{
				Thread.Sleep(DefaultWaitTime);
			}
			Assert.AreEqual(33, result1);
		}

		private IQueue<T> CreateQueue<T>()
		{
			return new MultithreadedQueue<T>();
		}

		private void PopNumbers(int limit, IQueue<int> queue)
		{
			for (int i = 0; i < limit; i++)
			{
				queue.Pop();
			}
		}

		private void PushNumbers(int from, int to, IQueue<int> queue)
		{
			for (int i = from; i <= to; i++)
			{
				queue.Push(i);
			}
		}
	}
}
