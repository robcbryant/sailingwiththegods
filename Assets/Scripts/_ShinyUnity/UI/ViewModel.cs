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
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Collections;

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

	public BoundModel(INotifyPropertyChanged source, string property, Func<object, T> adaptor = null) 
	{
		Adaptor = adaptor;
		Bind(source, property);
	}

	public void Bind(INotifyPropertyChanged source, string property) {

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

public class CompoundWrapperModel<TIn1, TIn2, TOut> : Model, IValueModel<TOut>
{
	// this is a Select that adds a dependency on a second model. it only works because it's a select

	EventOwner Owner = new EventOwner();
	DelegateHandle Handle1;
	DelegateHandle Handle2;

	IValueModel<TIn1> Source1;
	IValueModel<TIn2> Source2;
	Func<TIn1, TIn2, TOut> AdaptOut;

	// keep a local cache of the value. we'll get notified if it ever changes
	private TOut _value;
	public TOut Value {
		get => _value;
		set => throw new NotImplementedException("Cannot set a compound wrapper model. It's a mapping from 2 models into one read-only model.");
	}

	void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
		if (sender == Source1 || sender == Source2) {
			Refresh();
			NotifyAny();
		}
	}

	void Refresh() {
		_value = AdaptOut(Source1.Value, Source2.Value);
	}

	public void Dispose() {
		Owner.Dispose();
		Handle1.Dispose();
		Handle2.Dispose();
	}

	public CompoundWrapperModel(IValueModel<TIn1> source1, IValueModel<TIn2> source2, Func<TIn1, TIn2, TOut> adaptOut) {
		AdaptOut = adaptOut;

		Bind(source1, source2);
	}

	public void Bind(IValueModel<TIn1> source1, IValueModel<TIn2> source2) {
		if (source1 == null || source2 == null) Debug.LogError("Tried to bind a model to a null source model.");

		Source1 = source1;
		Source2 = source2;

		Owner.Unsubscribe(Handle1);
		Owner.Unsubscribe(Handle2);
		Handle1 = Owner.Subscribe(() => source1.PropertyChanged += OnPropertyChanged, () => source1.PropertyChanged -= OnPropertyChanged);
		Handle2 = Owner.Subscribe(() => source2.PropertyChanged += OnPropertyChanged, () => source2.PropertyChanged -= OnPropertyChanged);

		Refresh();
		NotifyAny();
	}
}

public static class ValueModel
{
	public static ValueModel<T> New<T>(T value) => new ValueModel<T>(value);
	public static WrapperModel<T, T> Wrap<T>(IValueModel<T> value) => new WrapperModel<T, T>(value, o => o, o => o);
	public static ICollectionModel<T> Wrap<T>(ObservableCollection<T> source) => new CollectionWrapperModel<T>(source);

	// adaptors
	public static IValueModel<string> AsString<T>(this IValueModel<T> self) => new WrapperModel<T, string>(self, o => o.ToString(), o => throw new NotImplementedException("AsString is readonly"));
	public static IValueModel<TOut> Select<T, TOut>(this IValueModel<T> self, Func<T, TOut> adaptor) => new WrapperModel<T, TOut>(self, o => adaptor(o), o => throw new NotImplementedException("Select is readonly"));
	public static IValueModel<TOut> Select<TIn1, TIn2, TOut>(this IValueModel<TIn1> self, IValueModel<TIn2> second, Func<TIn1, TIn2, TOut> adaptor) => new CompoundWrapperModel<TIn1, TIn2, TOut>(self, second, (o1, o2) => adaptor(o1, o2));

	// collection wrappers
	public static CollectionWrapperModel<T, TOut, int> Select<T, TOut>(this ICollectionModel<T> self, Func<T, TOut> adaptor) => new CollectionWrapperModel<T, TOut, int>(self, o => true, o => adaptor(o), o => throw new NotImplementedException("Select is readonly"), o => 1);
	public static CollectionWrapperModel<T, T, int> Where<T>(this ICollectionModel<T> self, Func<T, bool> filter) => new CollectionWrapperModel<T, T, int>(self, o => filter(o), o => o, o => o, o => 1);
	public static CollectionWrapperModel<T, T, TOrderKey> OrderBy<T, TOrderKey>(this ICollectionModel<T> self, Func<T, TOrderKey> selector) => new CollectionWrapperModel<T, T, TOrderKey>(self, o => true, o => o, o => o, selector);
}

public interface ICollectionModel<T> : IValueModel<IEnumerable<T>>, ICollection<T>, INotifyCollectionChanged
{
}


public class CollectionWrapperModel<T> : Model, ICollectionModel<T>
{
	// this is needed because IValueModel doesn't make sense for collections...
	public event NotifyCollectionChangedEventHandler CollectionChanged;

	// lazy evaluation of the linq makes this work on every call
	IEnumerable<T> Data => Source;

	public IEnumerable<T> Value { get => Data; set => throw new NotImplementedException("Cannot set. CollectionWrapper is read-only."); }

	EventOwner Owner = new EventOwner();
	DelegateHandle Handle;

	ObservableCollection<T> Source;

	void OnPropertyChanged(object sender, NotifyCollectionChangedEventArgs e) {
		if (sender == Source) {
			NotifyCollection(e);
			Notify();
		}
	}

	public void Dispose() {
		Owner.Dispose();
		Handle.Dispose();
	}

	public CollectionWrapperModel(ObservableCollection<T> source) {
		Bind(source);
	}

	public void Bind(ObservableCollection<T> source) {
		if (source == null) Debug.LogError("Tried to bind a model to a null source model.");

		Source = source;

		Owner.Unsubscribe(Handle);
		Handle = Owner.Subscribe(() => source.CollectionChanged += OnPropertyChanged, () => source.CollectionChanged -= OnPropertyChanged);

		NotifyAny();
		NotifyCollectionAny();
	}

	void NotifyCollection(NotifyCollectionChangedEventArgs e) {
		CollectionChanged?.Invoke(this, e);
	}

	void NotifyCollectionAny() {
		CollectionChanged?.Invoke(this, null);
	}

	#region ICollection<T> implementation

	public int Count => Value.Count();
	public bool IsReadOnly => false;

	public void Add(T obj) => Source.Add(obj);
	public bool Remove(T obj) => Source.Remove(obj);
	public void Clear() => Source.Clear();

	public IEnumerator<T> GetEnumerator() => Value.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => Value.GetEnumerator();

	public bool Contains(T item) => Value.Contains(item);
	public void CopyTo(T[] array, int arrayIndex) => Value.ToList().CopyTo(array, arrayIndex);

	#endregion
}

public class CollectionWrapperModel<TIn, TOut, TOrderKey> : Model, ICollectionModel<TOut>
{
	// this is needed because IValueModel doesn't make sense for collections...
	public event NotifyCollectionChangedEventHandler CollectionChanged;

	// TODO: it's a little unintutive that a CollectionWrapperModel breaks linq's laziness, but we need this so we can keep track of the old index
	// there must be a better way. this was inspired by: https://stackoverflow.com/questions/26784373/filter-and-update-a-readonlyobservablecollection
	IEnumerable<TOut> Data => Filtered
		.Select(o => AdaptOut(o))
		.OrderBy(OrderBy);

	IEnumerable<TIn> Filtered;
	IEnumerable<TIn> RecomputeFiltered() => Source.Value
		.Where(o => Filter(o))
		.ToList();

	public IEnumerable<TOut> Value { get => Data; set => throw new NotImplementedException("Cannot set. CollectionWrapper is read-only."); }

	EventOwner Owner = new EventOwner();
	DelegateHandle Handle;

	ICollectionModel<TIn> Source;
	Func<TIn, TOut> AdaptOut;
	Func<TOut, TIn> AdaptIn;
	Func<TIn, bool> Filter;
	Func<TOut, TOrderKey> OrderBy;

	void OnPropertyChanged(object sender, NotifyCollectionChangedEventArgs e) {
		if (sender == Source) {
			if((e.OldItems != null && e.OldItems.Cast<TIn>().Any(o => Filter(o))) || (e.NewItems != null && e.NewItems.Cast<TIn>().Any(o => Filter(o)))) {

				NotifyCollectionChangedEventArgs mappedArgs = null;

				// need to preserve the original filtered list for this block so we can figure out the old indexes
				var newFiltered = RecomputeFiltered();

				// observablecollection never has more than one item in each of its event args
				switch (e.Action) {
					case NotifyCollectionChangedAction.Add:
						mappedArgs = new NotifyCollectionChangedEventArgs(e.Action, AdaptOut((TIn)e.NewItems[0]), IndexOf(newFiltered, (TIn)e.NewItems[0]));
						break;
					case NotifyCollectionChangedAction.Move:
						mappedArgs = new NotifyCollectionChangedEventArgs(e.Action, AdaptOut((TIn)e.OldItems[0]), IndexOf(newFiltered, (TIn)e.OldItems[0]), IndexOf(Filtered, (TIn)e.OldItems[0]));
						break;
					case NotifyCollectionChangedAction.Remove:
						mappedArgs = new NotifyCollectionChangedEventArgs(e.Action, AdaptOut((TIn)e.OldItems[0]), IndexOf(Filtered, (TIn)e.OldItems[0]));
						break;
					case NotifyCollectionChangedAction.Replace:
						mappedArgs = new NotifyCollectionChangedEventArgs(e.Action, AdaptOut((TIn)e.NewItems[0]), AdaptOut((TIn)e.OldItems[0]), IndexOf(Filtered, (TIn)e.OldItems[0]));
						break;
					case NotifyCollectionChangedAction.Reset:
						mappedArgs = new NotifyCollectionChangedEventArgs(e.Action);
						break;
					default:
						Debug.LogError("Unsupported collection change in CollectionWrapperModel.");
						break;
				}

				Filtered = newFiltered;
				NotifyCollection(mappedArgs);
				Notify();
			}
		}
	}

	public int IndexOf(IEnumerable<TIn> haystack, TIn needle) {
		for(var i = 0; i < haystack.Count(); i++) {
			var curr = haystack.ElementAtOrDefault(i);
			if (ReferenceEquals(curr, needle) || EqualityComparer<TIn>.Default.Equals(curr, needle)) {
				return i;
			}
		}

		return -1;
	}

	public void Dispose() {
		Owner.Dispose();
		Handle.Dispose();
	}

	public CollectionWrapperModel(ICollectionModel<TIn> source, Func<TIn, bool> filter, Func<TIn, TOut> adaptOut, Func<TOut, TIn> adaptIn, Func<TOut, TOrderKey> orderBy) {
		AdaptOut = adaptOut;
		AdaptIn = adaptIn;
		Filter = filter;
		OrderBy = orderBy;

		Bind(source);
		Filtered = RecomputeFiltered();
	}

	public void Bind(ICollectionModel<TIn> source) {
		if (source == null) Debug.LogError("Tried to bind a model to a null source model.");

		Source = source;

		Owner.Unsubscribe(Handle);
		Handle = Owner.Subscribe(() => source.CollectionChanged += OnPropertyChanged, () => source.CollectionChanged -= OnPropertyChanged);

		NotifyAny();
		NotifyCollectionAny();
	}

	void NotifyCollection(NotifyCollectionChangedEventArgs e) {
		CollectionChanged?.Invoke(this, e);
	}

	void NotifyCollectionAny() {
		CollectionChanged?.Invoke(this, null);
	}

	#region ICollection<T> implementation

	public int Count => Value.Count();
	public bool IsReadOnly => false;

	public void Add(TOut obj) => Source.Add(AdaptIn(obj));
	public bool Remove(TOut obj) => Source.Remove(AdaptIn(obj));
	public void Clear() => Source.Clear();

	public IEnumerator<TOut> GetEnumerator() => Value.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => Value.GetEnumerator();

	public bool Contains(TOut item) => Value.Contains(item);
	public void CopyTo(TOut[] array, int arrayIndex) => Value.ToList().CopyTo(array, arrayIndex);

	#endregion
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

	public WrapperModel(IValueModel<TIn> source, Func<TIn, TOut> adaptOut, Func<TOut, TIn> adaptIn)
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
