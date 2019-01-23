using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Utils
{
	public static float GetRange(float Xinput, float Xmax, float Xmin, float Ymax, float Ymin) {

		return (((Xinput - Xmin) / (Xmax - Xmin)) * (Ymax - Ymin)) + Ymin;

	}

	public static bool FastApproximately(float a, float b, float threshold) {
		return ((a < b) ? (b - a) : (a - b)) <= threshold;
	}
}

#region [ReadOnly] Attribute

/// <summary>
/// Place [ReadOnly] attribute on fields that should be shown in the inspector for debug purposes but should not be editable.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class ReadOnlyAttribute : PropertyAttribute { }
#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeDrawer : UnityEditor.PropertyDrawer
{
	public override void OnGUI(Rect rect, UnityEditor.SerializedProperty prop, GUIContent label) {
		bool wasEnabled = GUI.enabled;
		GUI.enabled = false;
		UnityEditor.EditorGUI.PropertyField(rect, prop);
		GUI.enabled = wasEnabled;
	}
}
#endif

#endregion
