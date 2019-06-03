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

public class InteractableBehaviour : OwnerBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public UnityEvent PointerClick = new UnityEvent();
	public UnityEvent PointerDown = new UnityEvent();
	public UnityEvent PointerUp = new UnityEvent();
	public UnityEvent PointerEnter = new UnityEvent();
	public UnityEvent PointerExit = new UnityEvent();

	public virtual void OnPointerDown(PointerEventData eventData) {
		PointerDown.Invoke();
	}

	public virtual void OnPointerUp(PointerEventData eventData) {
		PointerUp.Invoke();
	}

	public virtual void OnPointerEnter(PointerEventData eventData) {
		PointerEnter.Invoke();
	}

	public virtual void OnPointerExit(PointerEventData eventData) {
		PointerExit.Invoke();
	}

	public virtual void OnPointerClick(PointerEventData eventData) {
		PointerClick.Invoke();
	}

	protected override void OnEnable() {
		Group.interactable = true;
	}

	protected override void OnDisable() {
		Group.interactable = false;
	}

	// text doesn't want to be made into a canvasgroup. it blocks clicks on its parent
	protected CanvasGroup Group {
		get {
			var group = gameObject.GetComponent<CanvasGroup>();
			if (group == null) {
				group = gameObject.AddComponent<CanvasGroup>();
			}
			return group;
		}
	}
}
