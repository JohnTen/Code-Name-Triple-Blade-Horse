using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SearchAndSelectPlus : EditorWindow
{
	string tagText = "";

	[MenuItem("Search Helper/Search && Select++")]
	static void Init()
	{
		GetWindow<SearchAndSelectPlus>().Show();
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical();
		IncreaseIndent();
		GUILayout.BeginHorizontal();
		tagText = EditorGUILayout.TagField("Tag: ", tagText);

		if (GUILayout.Button("Select"))
		{
			Selection.objects = GameObject.FindGameObjectsWithTag(tagText);
		}
		GUILayout.EndHorizontal();
		DecreaseIndent();
		GUILayout.EndVertical();
	}

	private void IncreaseIndent()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
	}

	private void DecreaseIndent()
	{
		GUILayout.EndHorizontal();
	}
}
