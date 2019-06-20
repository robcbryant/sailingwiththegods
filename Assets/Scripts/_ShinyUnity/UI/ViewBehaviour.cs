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
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class ViewBehaviour : OwnerBehaviour { }

// this is the code-behind for the UI. It binds the viewmodel to the view components, and registers events that call back to update the viewmodel.
// the viewmodel updates any other models and notifies anyone listening that it has changed
public abstract class ViewBehaviour<TModel> : ViewBehaviour where TModel : INotifyPropertyChanged
{
	DelegateHandle ModelSubscription;

	public RectTransform RectTransform => transform as RectTransform;

	public TModel Model { get; private set; }
	virtual public void Bind(TModel model) {
		Model = model;
		
		if (model == null)
		{
			Debug.LogWarning("Tried to bind view to a null model on " + name);
			return;
		}

		Unsubscribe(ModelSubscription);
		ModelSubscription = Subscribe(() => model.PropertyChanged += OnPropertyChanged, () => model.PropertyChanged -= OnPropertyChanged);

		Refresh();
	}

	void OnPropertyChanged(object sender, PropertyChangedEventArgs e) 
	{
		Refresh(sender, e?.PropertyName);
	}

	protected void Refresh() => Refresh(null, null);
	protected void Refresh(object sender) => Refresh(sender, null);
	virtual protected void Refresh(object sender, string propertyChanged) { }
}
