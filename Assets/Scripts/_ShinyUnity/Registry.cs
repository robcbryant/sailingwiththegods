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
using System.Threading.Tasks;

public class Registry
{
	Dictionary<Type, object> _objs = new Dictionary<Type, object>();

	public void Register<T>(T obj)
	{
		// hide the old one if replaced
		if(Has<T>())
		{
			_objs.Remove(typeof(T));
		}
		_objs.Add(typeof(T), obj);
	}

	// NOTE: THe obj passed in here isn't used. just there to make you think about this at call time
	public void Unregister<T>(T obj)
	{
		if(Has<T>())
		{
			_objs.Remove(typeof(T));
		}
	}

	public bool Has<T>() => _objs.ContainsKey(typeof(T));
	public T Get<T>()
	{
		if (_objs.ContainsKey(typeof(T)))
		{
			return (T)_objs[typeof(T)];
		}
		else return default;
	}
}

