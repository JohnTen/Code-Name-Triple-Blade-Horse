using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace JTUtility
{
	[CustomPropertyDrawer(typeof(PairedValue), true)]
	public class PairedValueDrawer : PropertyDrawer
	{
		SerializedProperty key, value;
		float keyHeight, valueHeight;
		const float gapBetweenKeyNValue = 4;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			key = property.FindPropertyRelative("key");
			value = property.FindPropertyRelative("value");
			if (key == null || value == null)
				return EditorGUIUtility.singleLineHeight;

			keyHeight = base.GetPropertyHeight(key, label);
			valueHeight = base.GetPropertyHeight(value, label);


			var totalHeight = Mathf.Max(keyHeight, valueHeight);

			return totalHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (key == null || value == null)
			{
				EditorGUI.HelpBox(position, property.type.ToString() + " is not a valid PairedValue.", MessageType.Error);
				return;
			}

			var left = position.xMin;
			var top = position.yMin;
			var width = position.width;

			var l_width = EditorGUIUtility.labelWidth;
			var l_Left = left;
			var k_width = (width + gapBetweenKeyNValue - l_width) / 2;
			var k_left = left + l_width;
			var v_width = k_width;
			var v_left = k_left + k_width + gapBetweenKeyNValue;

			top += EditorGUIUtility.standardVerticalSpacing;

			EditorGUI.LabelField(new Rect(l_Left, top, width, EditorGUIUtility.singleLineHeight), property.displayName);
			EditorGUI.PropertyField(new Rect(k_left, top, k_width, keyHeight), key, GUIContent.none, true);
			EditorGUI.PropertyField(new Rect(v_left, top, v_width, valueHeight), value, GUIContent.none, true);
		}
	}
}
