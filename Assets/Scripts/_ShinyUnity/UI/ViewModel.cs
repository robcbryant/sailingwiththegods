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
using System.Runtime.CompilerServices;
using System.ComponentModel;
using UnityEngine;

public class Model : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	public void Notify([CallerMemberName]string property = null) 
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
	}

	public void NotifyAny() 
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
	}
}

public class ListenerModel : Model, IDisposable
{
	EventOwner Owner = new EventOwner();
	List<DelegateHandle> Handles = new List<DelegateHandle>();

	void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
		NotifyAny();
	}

	public void Dispose() {
		Owner.Dispose();

		foreach(var handle in Handles) {
			handle.Dispose();
		}
	}

	/// <summary>
	/// Make this listener dispatch a null PropertyChanged whenever another model changes
	/// </summary>
	public void Listen(INotifyPropertyChanged source) {

		if (source == null) Debug.LogError("Tried to bind a model to a null source model.");
		
		var handle = Owner.Subscribe(() => source.PropertyChanged += OnPropertyChanged, () => source.PropertyChanged -= OnPropertyChanged);
		Handles.Add(handle);

		NotifyAny();
	}
}

public class BoundModel<T> : Model, IDisposable, IValueModel<T>
{
	EventOwner Owner = new EventOwner();
	DelegateHandle Handle;

	string SourceProperty;
	INotifyPropertyChanged Source;
	Func<object, T> Adaptor;

	// keep a local cache of the value. we'll get notified if it ever changes
	private T _value;
	public T Value {
		get => _value;
		set {
			_value = value;

			// setting this should dispatch notify for us
			Source
				.GetType()
				.GetProperty(SourceProperty)
				.SetValue(Source, value);
		}
	}

	void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (sender == Source && (e.PropertyName == null || e.PropertyName == SourceProperty)) 
		{
			Refresh();
			Notify(e.PropertyName);
		}
	}

	void Refresh() 
	{
		var val = Source
				.GetType()
				.GetProperty(SourceProperty)
				.GetValue(Source);

		_value = Adaptor != null ? Adaptor(val) : (T)val;
	}

	public void Dispose()
	{
		Owner.Dispose();
		Handle.Dispose();
	}

	public BoundModel(INotifyPropertyChanged source, string property = null, Func<object, T> adaptor = null) 
	{
		Adaptor = adaptor;
		Bind(source, property);
	}

	public void Bind(INotifyPropertyChanged source, string property = null) {

		if (source == null) Debug.LogError("Tried to bind a model to a null source model.");

		Source = source;
		SourceProperty = property;

		Owner.Unsubscribe(Handle);
		Handle = Owner.Subscribe(() => source.PropertyChanged += OnPropertyChanged, () => source.PropertyChanged -= OnPropertyChanged);

		Refresh();
		NotifyAny();
	}
}

public interface IValueModel<T> : INotifyPropertyChanged
{
	T Value { get; set; }
}

public static class ValueModel
{
	public static ValueModel<T> New<T>(T value) => new ValueModel<T>(value);

	// adaptors
	public static IValueModel<string> AsString<T>(this IValueModel<T> self) => new WrapperModel<T, string>(self, o => o.ToString());
}

public class WrapperModel<TIn, TOut> : Model, IValueModel<TOut>
{
	EventOwner Owner = new EventOwner();
	DelegateHandle Handle;

	IValueModel<TIn> Source;
	Func<TIn, TOut> AdaptOut;
	Func<TOut, TIn> AdaptIn;

	// keep a local cache of the value. we'll get notified if it ever changes
	private TOut _value;
	public TOut Value {
		get => _value;
		set {
			_value = value;

			// setting this should dispatch notify for us
			Source.Value = AdaptIn(value);
		}
	}

	void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
		if (sender == Source) {
			Refresh();
			NotifyAny();
		}
	}

	void Refresh() {
		_value = AdaptOut(Source.Value);
	}

	public void Dispose() {
		Owner.Dispose();
		Handle.Dispose();
	}

	public WrapperModel(IValueModel<TIn> source, Func<TIn, TOut> adaptOut = null, Func<TOut, TIn> adaptIn = null)
	{
		AdaptOut = adaptOut;
		AdaptIn = adaptIn;

		Bind(source);
	}

	public void Bind(IValueModel<TIn> source)
	{
		if (source == null) Debug.LogError("Tried to bind a model to a null source model.");

		Source = source;

		Owner.Unsubscribe(Handle);
		Handle = Owner.Subscribe(() => source.PropertyChanged += OnPropertyChanged, () => source.PropertyChanged -= OnPropertyChanged);

		Refresh();
		NotifyAny();
	}
}

public class ValueModel<T> : Model, IValueModel<T>
{
	private T _value;
	public T Value {
		get => _value;
		set { _value = value; Notify(); }
	}

	public ValueModel(T value) 
	{
		Value = value;
	}
}
