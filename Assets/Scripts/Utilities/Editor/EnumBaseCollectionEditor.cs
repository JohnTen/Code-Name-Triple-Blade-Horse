using System.Reflection;
using UnityEngine;
using UnityEditor;
using JTUtility;

namespace JTUtility.Editor
{
	[CustomPropertyDrawer(typeof(EnumBaseCollection), true)]
	public class EnumBaseCollectionEditor : PropertyDrawer
	{
		protected SerializedProperty keys, values;
		protected int length;
		float[] keyHeights;
		float[] valueHeights;
		float[] elementHeights;
		string[] enumNames;
		const float lineHeight = 16f;
		const float indent = 10f;

		public override float GetPropertyHeight(
			SerializedProperty property, GUIContent label)
		{
			keys = property.FindPropertyRelative("keys");
			values = property.FindPropertyRelative("vals");
			if (keys == null || values == null)
				return 16f;
			length = keys.arraySize;
			if (values.arraySize < length)
				length = values.arraySize;
			keyHeights = new float[length];
			valueHeights = new float[length];
			elementHeights = new float[length];
			enumNames = keys.enumDisplayNames;
			var totalHeight = 0f;
			int i;
			for (i = 0; i < length; i++)
			{
				var thisHeight = EditorGUI.GetPropertyHeight(
					keys.GetArrayElementAtIndex(i), new GUIContent(""), true);
				var valueHeight = EditorGUI.GetPropertyHeight(
					values.GetArrayElementAtIndex(i), new GUIContent(""), true);
				keyHeights[i] = thisHeight;
				valueHeights[i] = valueHeight;
				if (valueHeight > thisHeight)
					thisHeight = valueHeight;
				thisHeight += 2f;
				totalHeight += thisHeight;
				elementHeights[i] = thisHeight;
			}
			return lineHeight +
				(property.isExpanded ? totalHeight + 8f : 0f);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (keys == null || values == null)
			{
				EditorGUI.HelpBox(position, property.type.ToString() + " is not a valid EnumBaseCollection.", MessageType.Error);
				return;
			}

			var left = position.xMin;
			var top = position.yMin;
			var width = position.width;

			EditorGUI.PropertyField(new Rect(left, top, width, lineHeight), property, false);
			top += lineHeight;

			if (property.isExpanded)
			{
				var k_width = width / 2;
				var totalIndent = (EditorGUI.indentLevel * indent);
				var k_left = left + totalIndent;
				var v_left = left + k_width;
				var v_width = k_width - totalIndent;

				v_width -= 4f;
				k_left += 2f;
				k_width -= 4f;
				v_left += 2f;
				top += 4f;

				int i;
				for (i = 0; i < length; i++)
				{
					var i_value = values.GetArrayElementAtIndex(i);
					EditorGUI.LabelField(new Rect(k_left, top, k_width, keyHeights[i]), enumNames[i]);
					ValueField(new Rect(v_left, top, v_width, valueHeights[i]), i_value, i);
					top += elementHeights[i];
				}
			}
		}
		
		public virtual void ValueField(Rect position,
			SerializedProperty property, int index)
		{
			if (property.isExpanded)
			{
				position.yMax = position.yMin + lineHeight - 1;
				EditorGUI.PropertyField(position, property, GUIContent.none);

				var totalIndent = (EditorGUI.indentLevel * indent);
				var width = position.width - totalIndent - 2f;
				var labelLeft = position.xMin - width - totalIndent -2f;

				var enumerator = property.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var top = position.yMin += lineHeight + 2;
					position.yMax = position.yMin + lineHeight - 1;
					EditorGUI.LabelField(new Rect(labelLeft, top, width, lineHeight), property.name);
					ValueField(position, (SerializedProperty)enumerator.Current, index + 1);
				}
			}
			else
			{
				EditorGUI.PropertyField(position, property, GUIContent.none);
			}
		}
	}
}
