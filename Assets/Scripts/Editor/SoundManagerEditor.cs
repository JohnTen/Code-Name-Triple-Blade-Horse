using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    private ReorderableList clipList;

    private void OnEnable()
    {
        clipList = new ReorderableList(serializedObject,
               serializedObject.FindProperty("clips"),
               true, true, true, true);

        clipList.elementHeightCallback =
            (int index) =>
            {
                var element = clipList.serializedProperty.GetArrayElementAtIndex(index);

                var height =
                    EditorGUI.GetPropertyHeight(element, true);

                return height;
            };

        clipList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = clipList.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, element, true);
                EditorGUI.indentLevel--;
            };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        clipList.DoLayoutList();
        EditorGUILayout.LabelField("Debug");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("soundObject"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sources"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
