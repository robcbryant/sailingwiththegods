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

/// <summary>
/// Takes ownership of unity objects and IDisposables so it can dispose them all at once during Dispose.
/// Gracefully handles adding and removing twice when adding/removing the same object instance.
/// </summary>
public class Owner : IDisposable
{
	HashSet<UnityEngine.Object> OwnedUnityObjs = new HashSet<UnityEngine.Object>();
	HashSet<IDisposable> OwnedDisposables = new HashSet<IDisposable>();

	/// <summary>
	/// Take ownership of this Unity object. It will be automatically destroyed on Dispose.
	/// </summary>
	public UnityEngine.Object Own(UnityEngine.Object obj)
	{
		OwnedUnityObjs.Add(obj);
		return obj;
	}

	/// <summary>
	/// Take ownership of this IDisposable. It will be automatically Disposed on Dispose.
	/// </summary>
	public IDisposable Own(IDisposable obj)
	{
		OwnedDisposables.Add(obj);
		return obj;
	}

	/// <summary>
	/// Transfer ownership of this Unity object to someone else. It will no longer be tracked by this group.
	/// </summary>
	public UnityEngine.Object Move(UnityEngine.Object obj)
	{
		OwnedUnityObjs.Remove(obj);
		return obj;
	}

	/// <summary>
	/// Transfer ownership of this IDisposable to someone else. It will no longer be tracked by this group.
	/// </summary>
	public IDisposable Move(IDisposable obj)
	{
		OwnedDisposables.Remove(obj);
		return obj;
	}

	/// <summary>
	/// Dispose every resource owned by this group.
	/// </summary>
	public void Dispose()
	{
		foreach (var obj in OwnedUnityObjs)
		{
			UnityEngine.Object.Destroy(obj);
		}
		OwnedUnityObjs.Clear();

		foreach (var obj in OwnedDisposables)
		{
			obj.Dispose();
		}
		OwnedDisposables.Clear();
	}
}
