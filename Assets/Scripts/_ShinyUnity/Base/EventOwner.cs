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
/// Tracks event listeners for you so you don't have to clean up listeners yourself.
/// Works with will miller events, and UnityEvents.
/// Gracefully handles duplicate subscribe calls. An identical handler function ref will not be added twice.
/// </summary>
public class EventOwner : IDisposable
{
	interface IUnityEventInfo
	{
		void Subscribe();
		void Unsubscribe();
	}

	struct UnityEventInfo<T> : IUnityEventInfo
	{
		public UnityEvent<T> Event;
		public UnityAction<T> Action;

		public void Subscribe()
		{
			Event.AddListener(Action);
		}

		public void Unsubscribe()
		{
			Event.RemoveListener(Action);
		}
	}

	struct UnityEventInfo : IUnityEventInfo
	{
		public UnityEvent Event;
		public UnityAction Action;

		public void Subscribe()
		{
			Event.AddListener(Action);
		}

		public void Unsubscribe()
		{
			Event.RemoveListener(Action);
		}
	}

	struct EventInfo
	{
		public System.Type Type;
		public Events.EventDelegate Delegate;
	}

	HashSet<IUnityEventInfo> UnityEvents = new HashSet<IUnityEventInfo>();
	Dictionary<System.Delegate, EventInfo> Delegates = new Dictionary<System.Delegate, EventInfo>();
	HashSet<DelegateHandle> DelegateHandles = new HashSet<DelegateHandle>();

	public void Subscribe<T>(UnityEvent<T> e, UnityAction<T> action)
	{
		var info = new UnityEventInfo<T> { Event = e, Action = action };
		if (!UnityEvents.Contains(info))
		{
			e.AddListener(action);
			UnityEvents.Add(info);
		}
	}

	public void Unsubscribe<T>(UnityEvent<T> e, UnityAction<T> action)
	{
		var info = new UnityEventInfo<T> { Event = e, Action = action };
		if (UnityEvents.Contains(info))
		{
			e.RemoveListener(action);
			UnityEvents.Remove(info);
		}
	}

	public void Subscribe(UnityEvent e, UnityAction action)
	{
		var info = new UnityEventInfo { Event = e, Action = action };
		if (!UnityEvents.Contains(info))
		{
			e.AddListener(action);
			UnityEvents.Add(info);
		}
	}

	public void Unsubscribe(UnityEvent e, UnityAction action)
	{
		var info = new UnityEventInfo { Event = e, Action = action };
		if (UnityEvents.Contains(info))
		{
			e.RemoveListener(action);
			UnityEvents.Remove(info);
		}
	}

	public void Subscribe<T>(Events.EventDelegate<T> del) where T : GameEvent
	{
		if (!Delegates.ContainsKey(del))
		{
			var info = new EventInfo
			{
				Type = typeof(T),
				Delegate = (e) => del((T)e)
			};

			Delegates.Add(del, info);
			Events.Instance.AddListener(info.Type, info.Delegate);
		}
	}

	public void Unsubscribe<T>(Events.EventDelegate<T> del) where T : GameEvent
	{
		if (Delegates.ContainsKey(del))
		{
			var info = Delegates[del];
			Events.Instance.RemoveListener(info.Type, info.Delegate);
			Delegates.Remove(del);
		}
	}
	
	public DelegateHandle Subscribe(Action subscribe, Action unsubscribe)
	{
		var handle = new DelegateHandle(subscribe, unsubscribe);
		DelegateHandles.Add(handle);
		return handle;
	}

	public void Unsubscribe(DelegateHandle handle) 
	{
		if(handle != null) 
		{
			handle.Dispose();
			DelegateHandles.Remove(handle);
		}
	}

	public void Enable()
	{
		foreach (var e in UnityEvents)
		{
			e.Subscribe();
		}
		foreach (var kvp in Delegates)
		{
			Events.Instance.AddListener(kvp.Value.Type, kvp.Value.Delegate);
		}
		foreach(var d in DelegateHandles) 
		{
			d.Enable();
		}
	}

	public void Disable()
	{
		foreach (var e in UnityEvents)
		{
			e.Unsubscribe();
		}
		foreach (var kvp in Delegates)
		{
			Events.Instance.RemoveListener(kvp.Value.Type, kvp.Value.Delegate);
		}
		foreach (var d in DelegateHandles) 
		{
			d.Disable();
		}
	}

	public void Dispose()
	{
		Disable();
		UnityEvents.Clear();
		Delegates.Clear();
		DelegateHandles.Clear();
	}
}
