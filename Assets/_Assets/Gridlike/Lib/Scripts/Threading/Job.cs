using System;
using System.Collections.Generic;
using System.Threading;

namespace GridLike
{
	/// <summary>
	/// Describes the state of a future.
	/// </summary>
	public enum JobState
	{
		/// <summary>
		/// The future hasn't begun to resolve a value.
		/// </summary>
		Pending,

		/// <summary>
		/// The future is working on resolving a value.
		/// </summary>
		Processing,

		/// <summary>
		/// The future has a value ready.
		/// </summary>
		Success,

		/// <summary>
		/// The future failed to resolve a value.
		/// </summary>
		Error
	}

	/// <summary>
	/// Defines the interface of an object that can be used to track a future value.
	/// </summary>
	/// <typeparam name="T">The type of object being retrieved.</typeparam>
	public interface IJob<T>
	{
		/// <summary>
		/// Gets the state of the future.
		/// </summary>
		JobState state { get; }

		/// <summary>
		/// Gets the value if the State is Success.
		/// </summary>
		T value { get; }

		/// <summary>
		/// Gets the failure exception if the State is Error.
		/// </summary>
		Exception error { get; }


		IJob<T> OnComplete(JobCallback<T> callback);
	}

	/// <summary>
	/// Defines the signature for callbacks used by the future.
	/// </summary>
	/// <param name="future">The future.</param>
	public delegate void JobCallback<T>(bool failed, T result);

	/// <summary>
	/// An implementation of <see cref="IJob{T}"/> that can be used internally by methods that return futures.
	/// </summary>
	/// <remarks>
	/// Methods should always return the <see cref="IJob{T}"/> interface when calling code requests a future.
	/// This class is intended to be constructed internally in the method to provide a simple implementation of
	/// the interface. By returning the interface instead of the class it ensures the implementation can change
	/// later on if requirements change, without affecting the calling code.
	/// </remarks>
	/// <typeparam name="T">The type of object being retrieved.</typeparam>
	public sealed class Job<T> : IJob<T>
	{
		private volatile JobState _state;
		private T _value;
		private Exception _error;

		private JobCallback<T> callback;

		/// <summary>
		/// Gets the state of the future.
		/// </summary>
		public JobState state { get { return _state; } }

		/// <summary>
		/// Gets the value if the State is Success.
		/// </summary>
		public T value
		{
			get
			{
				if (_state != JobState.Success) {
					throw new InvalidOperationException("value is not available unless state is Success.");
				}

				return _value;
			}
		}

		/// <summary>
		/// Gets the failure exception if the State is Error.
		/// </summary>
		public Exception error
		{
			get
			{
				if (_state != JobState.Error) {
					throw new InvalidOperationException("error is not available unless state is Error.");
				}

				return _error;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Job{T}"/> class.
		/// </summary>
		public Job()
		{
			_state = JobState.Pending;
		}


		/// <summary>
		/// Adds a new callback to invoke if the future value is retrieved successfully or has an error.
		/// </summary>
		/// <param name="callback">The callback to invoke.</param>
		/// <returns>The future so additional calls can be chained together.</returns>
		public IJob<T> OnComplete(JobCallback<T> callback)
		{
			if (_state == JobState.Success) {
				if (ThreadManager.isMainThread) {
					callback(false, value);
				}
				else {
					ThreadManager.InvokeAsync(() => callback(false, value));
				}
			} else if(_state == JobState.Error) {
				if(ThreadManager.isMainThread) {
					callback(true, value);
				} else {
					ThreadManager.InvokeAsync(() => callback(true, value));
				}
			} else {
				this.callback = callback;
			}

			return this;
		}

		/// <summary>
		/// Begins running a given function on a background thread to resolve the future's value, as long
		/// as it is still in the Pending state.
		/// </summary>
		/// <param name="func">The function that will retrieve the desired value.</param>
		public IJob<T> Process(Func<T> func)
		{
			if (_state != JobState.Pending) {
				throw new InvalidOperationException("Cannot process a future that isn't in the Pending state.");
			}

			_state = JobState.Processing;

			ThreadPool.QueueUserWorkItem(_ =>
				{
					try {
						// Directly call the Impl version to avoid the state validation of the public method
						AssignImpl(func());
					}
					catch (Exception e) {
						// Directly call the Impl version to avoid the state validation of the public method
						FailImpl(e);
					}
				});

			return this;
		}

		/// <summary>
		/// Allows manually assigning a value to a future, as long as it is still in the pending state.
		/// </summary>
		/// <remarks>
		/// There are times where you may not need to do background processing for a value. For example,
		/// you may have a cache of values and can just hand one out. In those cases you still want to
		/// return a future for the method signature, but can just call this method to fill in the future.
		/// </remarks>
		/// <param name="value">The value to assign the future.</param>
		public void Assign(T value)
		{
			if (_state != JobState.Pending) {
				throw new InvalidOperationException("Cannot assign a value to a future that isn't in the Pending state.");
			}

			AssignImpl(value);
		}

		/// <summary>
		/// Allows manually failing a future, as long as it is still in the pending state.
		/// </summary>
		/// <remarks>
		/// As with the Assign method, there are times where you may know a future value is a failure without
		/// doing any background work. In those cases you can simply fail the future manually and return it.
		/// </remarks>
		/// <param name="error">The exception to use to fail the future.</param>
		public void Fail(Exception error)
		{
			if (_state != JobState.Pending) {
				throw new InvalidOperationException("Cannot fail future that isn't in the Pending state.");
			}

			FailImpl(error);
		}

		private void AssignImpl(T value)
		{
			_value = value;
			_error = null;
			_state = JobState.Success;

			ThreadManager.InvokeAsync(FlushSuccessCallback);
		}

		private void FailImpl(Exception error)
		{
			_value = default(T);
			_error = error;
			_state = JobState.Error;

			ThreadManager.InvokeAsync(FlushErrorCallback);
		}

		private void FlushSuccessCallback()
		{
			callback(true, value);
		}

		private void FlushErrorCallback()
		{
			callback(false, value);
		}
	}
}
