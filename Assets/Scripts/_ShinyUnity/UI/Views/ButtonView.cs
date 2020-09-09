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
using UnityEngine;
using UnityEngine.UI;

public class ButtonViewModel : Model
{
	private string _Label;
	public string Label { get => _Label; set { _Label = value; Notify(); } }

	private Action _OnClick;
	public Action OnClick { get => _OnClick; set { _OnClick = value; Notify(); } }
}

public class ButtonView : ViewBehaviour<IValueModel<ButtonViewModel>>
{
	[SerializeField] StringView Label = null;
	[SerializeField] Button Button = null;
	
	private void Start() {
		Subscribe(Button.onClick, OnClick);
	}

	public override void Bind(IValueModel<ButtonViewModel> model) {
		base.Bind(model);

		if (Label == null) {
			Label = GetComponentInChildren<StringView>();
		}
		if (Button == null) {
			Button = GetComponent<Button>();
		}

		if (model == null) 
		{
			Debug.LogWarning("Tried to bind view to a null model on " + name);
			return;
		}

		if(Model.Value.Label != null) {
			Label?.Bind(new BoundModel<string>(Model.Value, nameof(Model.Value.Label)));
		}
	}

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		// allow the ButtonViewModel contained in the IValueModel wrapper to be changed to a new instance and have the label update
		if(sender == Model) {
			if (Model.Value.Label != null) {
				Label?.Bind(new BoundModel<string>(Model.Value, nameof(Model.Value.Label)));
			}
		}
	}

	void OnClick() {
		Model?.Value?.OnClick?.Invoke();
	}

	public bool Interactable {
		get {
			return Button.interactable;
		}
		set {
			Button.interactable = value;
		}
	}
}
