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
			lock(_locker)
			{
				_queue.Enqueue(data);
				Monitor.Pulse(_locker);
			}
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the queue
		/// </summary>
		public T Pop()
		{
			lock(_locker)
			{

				while (_queue.Count == 0)
				{
					Monitor.Wait(_locker);
				}

				return (T)_queue.Dequeue();
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
			lock (_locker)
			{
				return _queue.Count;
			}
		}
	}
}