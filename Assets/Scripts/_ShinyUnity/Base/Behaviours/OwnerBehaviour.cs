// The MIT License (MIT)
// 
// Copyright (c) 2018 Shiny Dolphin Games LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A MonoBehaviour that manages the lifetime and ownership of resources it allocates and automatically cleans up after itself OnDestroy.
/// It also cleans up event listeners assigned through its interface, and automatically enables/disables the events in OnEnable/OnDisable.
/// </summary>
public abstract class OwnerBehaviour : MonoBehaviour
{
	List<Action> Cleanups = new List<Action>();
	Owner Owned = new Owner();
	EventOwner UnityEventOwner = new EventOwner();
	Registry Registry = new Registry();

	#region Unity Messages

	protected virtual void OnEnable()
	{
		UnityEventOwner.Enable();
	}

	protected virtual void OnDisable()
	{
		UnityEventOwner.Disable();
	}

	protected virtual void OnDestroy()
	{
		Owned.Dispose();
		UnityEventOwner.Dispose();

		foreach (var todo in Cleanups)
		{
			todo();
		}
	}

	#endregion

	#region Events

	protected void Subscribe(UnityEvent e, UnityAction action)
	{
		UnityEventOwner.Subscribe(e, action);
	}

	protected void Unsubscribe(UnityEvent e, UnityAction action)
	{
		UnityEventOwner.Unsubscribe(e, action);
	}

	protected void Subscribe<T>(UnityEvent<T> e, UnityAction<T> action)
	{
		UnityEventOwner.Subscribe<T>(e, action);
	}

	protected void Unsubscribe<T>(UnityEvent<T> e, UnityAction<T> action)
	{
		UnityEventOwner.Unsubscribe<T>(e, action);
	}

	protected void Subscribe<T>(Events.EventDelegate<T> del) where T : GameEvent
	{
		UnityEventOwner.Subscribe(del);
	}

	protected void Unsubscribe<T>(Events.EventDelegate<T> del) where T : GameEvent
	{
		UnityEventOwner.Unsubscribe(del);
	}

	protected DelegateHandle Subscribe(Action subscribe, Action unsubscribe) 
	{
		return UnityEventOwner.Subscribe(subscribe, unsubscribe);
	}

	public void Unsubscribe(DelegateHandle handle) 
	{
		UnityEventOwner.Unsubscribe(handle);
	}

	#endregion

	#region Generic Cleanup (very similar to C++ destructors)

	protected void DoOnDestroy(Action task)
	{
		Cleanups.Add(task);
	}

	protected void SetupAndDoOnDestroy(Action setup, Action onDestroy)
	{
		setup();
		DoOnDestroy(onDestroy);
	}

	#endregion

	#region Object Ownership

	/// <summary>
	/// Take ownership of this Unity object. It will be automatically destroyed with the MonoBehaviour.
	/// </summary>
	protected UnityEngine.Object Own(UnityEngine.Object obj)
	{
		return Owned.Own(obj);
	}

	/// <summary>
	/// Take ownership of this IDisposable. It will be automatically Disposed with the MonoBehaviour.
	/// </summary>
	protected IDisposable Own(IDisposable obj)
	{
		return Owned.Own(obj);
	}

	/// <summary>
	/// Transfer ownership of this Unity object to someone else. It will no longer be tracked by this MonoBehaviour.
	/// </summary>
	protected UnityEngine.Object Move(UnityEngine.Object obj)
	{
		return Owned.Move(obj);
	}

	/// <summary>
	/// Transfer ownership of this IDisposable to someone else. It will no longer be tracked by this MonoBehaviour.
	/// </summary>
	protected IDisposable Move(IDisposable obj)
	{
		return Owned.Move(obj);
	}

	#endregion

	#region Registry

	public void Register<T>(T obj)
	{
		Registry.Register(obj);
	}

	public void Unregister<T>(T obj)
	{
		Registry.Unregister(obj);
	}

	public bool Has<T>() => Registry.Has<T>();
	public T Get<T>()
	{
		return Registry.Get<T>();
	}

	#endregion
}
