﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

// Taken and lightly altered version of UnityToolbag's Dispatcher system. 

/// <summary>
/// A system for launching simple worker task on background thread.
/// </summary>
public class ThreadManager : MonoBehaviour
{
	private static ThreadManager _instance;

	// We can't use the behaviour reference from other threads, so we use a separate bool
	// to track the instance so we can use that on the other threads.
	private static bool _instanceExists;

	private static Thread _mainThread;
	private static object _lockObject = new object();
	private static readonly Queue<Action> _actions = new Queue<Action>();

	public static bool instanceExists { get { return _instanceExists; } }

	/// <summary>
	/// Gets a value indicating whether or not the current thread is the game's main thread.
	/// </summary>
	public static bool isMainThread
	{
		get
		{
			return Thread.CurrentThread == _mainThread;
		}
	}

	/// <summary>
	/// Queues an action to be invoked on the main game thread.
	/// </summary>
	/// <param name="action">The action to be queued.</param>
	public static void InvokeAsync(Action action)
	{
		if (!_instanceExists) {
			Debug.LogError("No Dispatcher exists in the scene. Actions will not be invoked!");
			return;
		}

		if (isMainThread) {
			// Don't bother queuing work on the main thread; just execute it.
			action();
		}
		else {
			lock (_lockObject) {
				_actions.Enqueue(action);
			}
		}
	}

	/// <summary>
	/// Queues an action to be invoked on the main game thread and blocks the
	/// current thread until the action has been executed.
	/// </summary>
	/// <param name="action">The action to be queued.</param>
	public static void Invoke(Action action)
	{
		if (!_instanceExists) {
			Debug.LogError("No Dispatcher exists in the scene. Actions will not be invoked!");
			return;
		}

		bool hasRun = false;

		InvokeAsync(() =>
			{
				action();
				hasRun = true;
			});

		// Lock until the action has run
		while (!hasRun) {
			Thread.Sleep(5);
		}
	}

	void Awake()
	{
		if (_instance) {
			DestroyImmediate(this);
		} else {
			_instance = this;
			_instanceExists = true;
			_mainThread = Thread.CurrentThread;
		}
	}

	void OnDestroy()
	{
		if (_instance == this) {
			_instance = null;
			_instanceExists = false;
		}
	}

	void Update()
	{
		lock (_lockObject) {
			while (_actions.Count > 0) {
				_actions.Dequeue()();
			}
		}
	}

	/// <summary>
	/// Creates a job. Only to be called on the main thread.
	/// </summary>
	/// <param name="action">The function to be run in the background thread.</param>
	/// <param name="callback">The callback to be called once the job is completed.</param>
	/// <typeparam name="T">The type of the data returned by the job.</typeparam>
	public static void CreateJob<T>(Func<T> action, JobCallback<T> callback) {

#if UNITY_WEBGL && !UNITY_EDITOR
		callback(false, action());
#else
		if(Application.isPlaying) {
			if (_instance == null) {
				GameObject go = GameObject.Find ("Gridlike");
				if (go == null) go = new GameObject ("Gridlike");

				ThreadManager man = go.AddComponent<ThreadManager> ();
				_instance = man;
				_instanceExists = true;
				_mainThread = Thread.CurrentThread;
			}

			Job<T> job = new Job<T>();

			job.OnComplete(callback);

			job.Process(action);
		} else {
			callback(false, action());
		}
#endif
	}
}