using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SoundClip))]
public class SoundClipDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUIUtility.singleLineHeight;
        var soundList = property.FindPropertyRelative("audioClips");
        var clipExpanded = property.FindPropertyRelative("clipExpanded");
        var settingExpanded = property.FindPropertyRelative("settingExpanded");
        var debugExpanded = property.FindPropertyRelative("debugExpanded");

        if (!clipExpanded.boolValue)
        {
            return height;
        }

        height += EditorGUIUtility.singleLineHeight; // Label
        height += EditorGUIUtility.singleLineHeight; // Group
        height += EditorGUIUtility.singleLineHeight; // Space between Label/group and settings
        height += EditorGUIUtility.singleLineHeight; // Space between sound clips

        height += EditorGUI.GetPropertyHeight(soundList, true); ;
        height += EditorGUIUtility.singleLineHeight;
        if (settingExpanded.boolValue)
            height += EditorGUIUtility.singleLineHeight * 6;

        height += EditorGUIUtility.singleLineHeight;
        if (debugExpanded.boolValue)
            height += EditorGUIUtility.singleLineHeight * 3;

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var soundList = property.FindPropertyRelative("audioClips");
        var clipExpanded = property.FindPropertyRelative("clipExpanded");
        var settingExpanded = property.FindPropertyRelative("settingExpanded");
        var debugExpanded = property.FindPropertyRelative("debugExpanded");

        position.height = EditorGUIUtility.singleLineHeight;

        label.text = property.FindPropertyRelative("group").stringValue + "/" + property.FindPropertyRelative("label").stringValue;
        clipExpanded.boolValue = EditorGUI.Foldout(position, clipExpanded.boolValue, label);
        position.center += Vector2.up * EditorGUIUtility.singleLineHeight;
        if (!clipExpanded.boolValue)
        {
            return;
        }

        EditorGUI.indentLevel++;

        EditorGUI.PropertyField(position, property.FindPropertyRelative("label"));
        position.center += Vector2.up * EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("group"));
        position.center += Vector2.up * EditorGUIUtility.singleLineHeight;
        position.center += Vector2.up * EditorGUIUtility.singleLineHeight;

        position.height = EditorGUI.GetPropertyHeight(soundList, GUIContent.none);
        EditorGUI.PropertyField(position, soundList, true);
        position.y += position.height;
        position.height = EditorGUIUtility.singleLineHeight;

        settingExpanded.boolValue = EditorGUI.Foldout(position, settingExpanded.boolValue, "Settings");
        position.y += EditorGUIUtility.singleLineHeight;
        if (settingExpanded.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("output"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("mute"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("playOnAwake"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("loop"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("volume"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("pitch"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.indentLevel--;
        }

        debugExpanded.boolValue = EditorGUI.Foldout(position, debugExpanded.boolValue, "Debug");
        position.center += Vector2.up * EditorGUIUtility.singleLineHeight;
        if (debugExpanded.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("standardVolume"));
            position.center += Vector2.up * EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("standardPitch"));
            position.center += Vector2.up * EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("source"));
            position.center += Vector2.up * EditorGUIUtility.singleLineHeight;
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
    }
}
