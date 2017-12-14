namespace QueueWrapper
{
	/// <summary>
	/// FIFO collection
	/// </summary>
	public interface IQueue<T>
	{
		void Push(T data);
		T Pop();
	}
}