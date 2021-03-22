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
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UISystem : MonoBehaviour
{
	private Canvas Canvas;

	private void Awake() {
		Canvas = GetComponent<Canvas>();
	}

	#region Generic cases for any view passed in

	protected void Add<T>(T c) where T : ViewBehaviour 
	{
		_Views.Add(typeof(T), c);
	}

	public T Show<T>(T view) where T : ViewBehaviour
	{
		view?.gameObject.SetActive(true);
		return view;
	}

	public T Show<T, TModel>(T view, TModel model)
		where T : ViewBehaviour<TModel>
		where TModel : INotifyPropertyChanged 
	{
		view.Bind(model);
		Show(view);
		return view;
	}

	public void Hide<T>(T view) where T : ViewBehaviour
	{
		view?.gameObject.SetActive(false);
	}

	public void HideAll() {
		foreach(var view in _Views) {
			Hide(view.Value);
		}
	}

	#endregion

	#region View registry system

	Dictionary<Type, ViewBehaviour> _Views = new Dictionary<Type, ViewBehaviour>();

	public IEnumerable<ViewBehaviour> GetActiveViews() => _Views.Values
		.Where(v => v != null && v.gameObject.activeSelf)
		.ToArray();

	public T Show<T>() where T : ViewBehaviour 
	{
		var view = Get<T>();
		if (view == null) Debug.LogError("No view " + typeof(T) + " is registered.");
		Show(view);
		return view;
	}

	public T Show<T, TModel>(TModel model) 
		where T : ViewBehaviour<TModel>
		where TModel : INotifyPropertyChanged	
	{
		var view = Get<T>();
		if (view == null) Debug.LogError("No view " + typeof(T) + " is registered.");
		Show(view, model);
		return view;
	}

	public void Hide<T>() where T : ViewBehaviour
	{
		Hide(Get<T>());
	}

	public void Toggle<T>() where T : ViewBehaviour
	{
		if(IsShown<T>())
		{
			Hide<T>();
		}
		else
		{
			Show<T>();
		}
	}

	public bool IsShown<T>() where T : ViewBehaviour
	{
		var result = Get<T>()?.gameObject.activeSelf;
		return result.HasValue ? result.Value : false;
	}

	public T Get<T>() where T : ViewBehaviour
	{
		if (_Views.ContainsKey(typeof(T)))
		{
			return _Views[typeof(T)] as T;
		}
		else
		{
			return null;
		}
	}

	public Vector3 WorldToUI(Camera worldCam, Vector3 worldPos) {
		//Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
		Vector3 screenPos = worldCam.WorldToScreenPoint(worldPos);
		Vector2 movePos;

		//Convert the screenpoint to ui rectangle local point
		RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, screenPos, Canvas.worldCamera, out movePos);
		//Convert the local point to world point
		return Canvas.transform.TransformPoint(movePos);
	}

	public static IEnumerable<RaycastResult> GetMouseOverUI() {
		var eventData = new PointerEventData(EventSystem.current);
		eventData.position = Input.mousePosition;

		var raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, raycastResults);
		return raycastResults.Where(r => r.module is GraphicRaycaster);
	}

	public static bool IsMouseOverUI() {
		return GetMouseOverUI().Count() > 0;
	}

	public static bool IsMouseOverUI(Graphic ui) {
		return GetMouseOverUI().Any(r => r.gameObject.GetComponent<Graphic>() == ui);
	}

	#endregion
}
