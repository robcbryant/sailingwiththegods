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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class ListView<TModel, TCellModel> : ViewBehaviour<TModel> 
	where TModel : INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable<TCellModel>
	where TCellModel : INotifyPropertyChanged
{
	[SerializeField] private Transform CellParent = null;
	[SerializeField] private GameObject CellPrefab = null;

	DelegateHandle CollectionModelSubscription;

	Queue<ViewBehaviour<TCellModel>> Pool = new Queue<ViewBehaviour<TCellModel>>();
	List<ViewBehaviour<TCellModel>> Used = new List<ViewBehaviour<TCellModel>>();

	private void PoolExistingObjects() {

		// only once at first scene load, add any zombie children to pool that aren't in the pool arleady. these were the initial ones in the scene at start
		if (!Pool.Any() && !Used.Any() && CellParent.childCount > 0) {
			var toDisable = new List<ViewBehaviour<TCellModel>>();
			for (var i = 0; i < CellParent.childCount; i++) {
				var child = CellParent.GetChild(i).GetComponent<ViewBehaviour<TCellModel>>();
				if (!Pool.Contains(child)) {
					toDisable.Add(child);
					Pool.Enqueue(child);
				}
			}

			foreach (var child in toDisable) {
				child.gameObject.SetActive(false);
			}
		}

	}

	void Clear()
	{
		PoolExistingObjects();

		foreach (var item in Used) {
			Pool.Enqueue(item);
			item.gameObject.SetActive(false);
		}
		Used.Clear();

	}

	void Insert(TCellModel cellModel, int idx) {
		ViewBehaviour<TCellModel> cell;
		if (!Pool.Any()) {
			cell = AllocateNewItem(cellModel);
		}
		else {
			cell = Pool.Dequeue();
		}

		cell.Bind(cellModel);
		cell.gameObject.SetActive(true);
		cell.transform.SetSiblingIndex(idx);

		if (idx >= Used.Count) {
			Used.Add(cell);
		}
		else {
			Used.Insert(idx, cell);
		}
	}

	TCellModel Remove(int index) {
		var cell = Used[index];
		cell.gameObject.SetActive(false);

		Used.RemoveAt(index);
		Pool.Enqueue(cell);

		return cell.Model;
	}

	ViewBehaviour<TCellModel> AllocateNewItem(TCellModel cellModel) {
		var cell = GameObject.Instantiate(CellPrefab).GetComponent<ViewBehaviour<TCellModel>>();
		cell.transform.SetParent(CellParent);
		cell.transform.localScale = Vector3.one;
		cell.gameObject.SetActive(false);
		return cell;
	}

	void Repopulate()
	{
		Clear();
		foreach (var cellModel in Model)
		{
			Insert(cellModel, Used.Count);
		}
	}

	public override void Bind(TModel model)
	{
		base.Bind(model);

		if(model == null) 
		{
			Debug.LogWarning("Tried to bind view to a null model on " + name);
			return;
		}

		Unsubscribe(CollectionModelSubscription);
		CollectionModelSubscription = Subscribe(() => model.CollectionChanged += OnCollectionChanged, () => model.CollectionChanged -= OnCollectionChanged);

		Repopulate();
	}

	void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		RefreshCollection(sender, e);
	}

	protected void RefreshCollection() => Repopulate();
	protected void RefreshCollection(object sender) => Repopulate();
	protected void RefreshCollection(object sender, NotifyCollectionChangedEventArgs e)
	{
		// observablecollection never has more than one item in each of its event args
		switch(e.Action) {
			case NotifyCollectionChangedAction.Add:
				Insert((TCellModel)e.NewItems[0], e.NewStartingIndex);
				break;
			case NotifyCollectionChangedAction.Move:
				var moving = Remove(e.OldStartingIndex);
				Insert(moving, e.NewStartingIndex);
				break;
			case NotifyCollectionChangedAction.Remove:
				Remove(e.OldStartingIndex);
				break;
			case NotifyCollectionChangedAction.Replace:
				Used[e.NewStartingIndex].Bind((TCellModel)e.NewItems[0]);
				break;
			case NotifyCollectionChangedAction.Reset:
				Repopulate();
				break;
			default:
				Debug.LogError("Unsupported collection change in ListView.");
				break;
		}
	}

	protected override void Refresh(object sender, string propertyChanged)
	{
		base.Refresh(sender, propertyChanged);

		// deliberately do nothing here. don't react to property changes on the list (such as count changing). we want to handle it efficiently in RefreshCollection
	}
}
