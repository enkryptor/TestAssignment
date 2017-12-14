using System.Collections;
using System.Threading;

namespace QueueWrapper
{
	/// <summary>
	/// Thread-safe FIFO collection
	/// </summary>
	public class MultithreadedQueue<T> : IQueue<T>
	{
		private readonly Queue _queue = new Queue();
		private readonly object _locker = new object();

		/// <summary>
		/// Adds an object to the end of the queue
		/// </summary>
		public void Push(T data)
		{
			var locked = false;
			try
			{
				Monitor.Enter(_locker, ref locked);
				_queue.Enqueue(data);
				Monitor.Pulse(_locker);
			}
			finally
			{
				if (locked) Monitor.Exit(_locker);
			}
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the queue
		/// </summary>
		public T Pop()
		{
			var locked = false;
			try
			{
				Monitor.Enter(_locker, ref locked);
				while (_queue.Count == 0)
				{
					Monitor.Wait(_locker);
				}

				var result = (T)_queue.Dequeue();
				return result;
			}
			finally
			{
				if (locked) Monitor.Exit(_locker);
			}
		}

		/// <summary>
		/// Returns the number of elements in the queue
		/// </summary>
		/// <remarks>
		/// Test purposes
		/// </remarks>
		public int Count()
		{
			var locked = false;
			try
			{
				Monitor.Enter(_locker, ref locked);
				return _queue.Count;
			}
			finally
			{
				if (locked) Monitor.Exit(_locker);
			}
		}
	}
}