using UnityEditor;
using UnityEngine;

namespace JTUtility.Editor
{
    [CustomPropertyDrawer(typeof(PairedValue), true)]
    public class PairedValueDrawer : PropertyDrawer
    {
        SerializedProperty key, value;
        float keyHeight, valueHeight;
        const float gapBetweenKeyNValue = 4;

        bool keyExpanded, valueExpanded;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            key = property.FindPropertyRelative("key");
            value = property.FindPropertyRelative("value");
            if (key == null || value == null)
                return EditorGUIUtility.singleLineHeight;

            keyHeight = EditorGUI.GetPropertyHeight(key, label, true);
            valueHeight = EditorGUI.GetPropertyHeight(value, label, true);

            var totalHeight = Mathf.Max(keyHeight, valueHeight);

            if (value.isExpanded || key.isExpanded)
                totalHeight += EditorGUIUtility.singleLineHeight;

            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (key == null || value == null)
            {
                EditorGUI.HelpBox(position, property.type.ToString() + " is not a valid PairedValue.", MessageType.Error);
                return;
            }

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var left = position.xMin;
            var top = position.yMin;
            var width = position.width;

            var l_width = EditorGUIUtility.labelWidth;
            var l_Left = left;
            var k_width = (width - gapBetweenKeyNValue - l_width) / 2;
            var k_left = left + l_width;
            var v_width = k_width;
            var v_left = k_left + k_width + gapBetweenKeyNValue;

            top += EditorGUIUtility.standardVerticalSpacing;

            if (keyExpanded && value.isExpanded)
            {
                key.isExpanded = false;
            }

            if (valueExpanded && key.isExpanded)
            {
                value.isExpanded = false;
            }
            keyExpanded = key.isExpanded;
            valueExpanded = value.isExpanded;

            EditorGUI.LabelField(new Rect(l_Left, top, l_width, lineHeight), property.displayName);

            if (keyHeight > lineHeight)
            {
                EditorGUI.PropertyField(new Rect(k_left, top, k_width, lineHeight), key);
            }
            else
            {
                EditorGUI.PropertyField(new Rect(k_left, top, k_width, lineHeight), key, GUIContent.none);
            }

            if (valueHeight > lineHeight)
            {
                EditorGUI.PropertyField(new Rect(v_left, top, v_width, lineHeight), value);
            }
            else
            {
                EditorGUI.PropertyField(new Rect(v_left, top, v_width, lineHeight), value, GUIContent.none);
            }

            if (keyExpanded || valueExpanded)
            {

                var rect = position;
                rect.height = keyExpanded ? keyHeight : valueHeight;
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, keyExpanded ? key : value, true);
            }
        }
    }
}
