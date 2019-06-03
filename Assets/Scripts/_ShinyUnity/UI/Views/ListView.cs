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
using UnityEngine;
using UnityEngine.UI;

// i click a delete button on a cell
// the cell model should have an ondelete callback
// the ondelete is set in the container, which decides what to do
// so the cell models need to be custom, use case specific. shouldn't just be generic
// so i really should make custom list subclasses i think

public abstract class ListView<TModel, TCellModel> : ViewBehaviour<TModel> 
	where TModel : INotifyCollectionChanged, INotifyPropertyChanged, ICollection<TCellModel>
	where TCellModel : INotifyPropertyChanged
{
	[SerializeField] private Transform CellParent;
	[SerializeField] private GameObject CellPrefab;

	DelegateHandle CollectionModelSubscription;

	void Clear()
	{
		for (var i = 0; i < CellParent.childCount; i++)
		{
			GameObject.Destroy(CellParent.GetChild(i).gameObject);
		}
	}

	void Repopulate()
	{
		Clear();
		foreach (var cellModel in Model)
		{
			var cell = GameObject.Instantiate(CellPrefab).GetComponent<ViewBehaviour<TCellModel>>();
			cell.Bind(cellModel);
			cell.transform.SetParent(CellParent);
		}
	}

	public override void Bind(TModel model)
	{
		base.Bind(model);

		Unsubscribe(CollectionModelSubscription);
		CollectionModelSubscription = Subscribe(() => model.CollectionChanged += OnCollectionChanged, () => model.CollectionChanged -= OnCollectionChanged);

		RefreshCollection(null, null);
	}

	void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		RefreshCollection(sender, e);
	}

	protected void RefreshCollection() => Refresh(null, null);
	protected void RefreshCollection(object sender) => Refresh(sender, null);
	protected void RefreshCollection(object sender, NotifyCollectionChangedEventArgs e)
	{
		// TODO: We have tons of granularity that we can use to improve performance. But for now, just clear and rebuild.
		Repopulate();
	}

	protected override void Refresh(object sender, string propertyChanged)
	{
		base.Refresh(sender, propertyChanged);

		Repopulate();
	}
}
