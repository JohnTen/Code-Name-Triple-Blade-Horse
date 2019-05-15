using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace JTUtility.Editor
{
	[CustomEditor(typeof(BoxArea))]
	class BoxAreaEditor : UnityEditor.Editor
	{
		BoxArea area;

		bool changing = false;

		private void OnEnable()
		{
			area = (BoxArea)target;
		}

		private void OnSceneGUI()
		{
			Handles.color = Color.white;
			ShowPoint(BoxCorner.UpperLeft);
			ShowPoint(BoxCorner.UpperRight);
			ShowPoint(BoxCorner.LowerLeft);
			ShowPoint(BoxCorner.LowerRight);
			
			Vector3 ulPoint = area.GetCornerPosition(BoxCorner.UpperLeft);
			Vector3 urPoint = area.GetCornerPosition(BoxCorner.UpperRight);
			Vector3 llPoint = area.GetCornerPosition(BoxCorner.LowerLeft);
			Vector3 lrPoint = area.GetCornerPosition(BoxCorner.LowerRight);

			Handles.DrawLine(ulPoint, urPoint);
			Handles.DrawLine(urPoint, lrPoint);
			Handles.DrawLine(lrPoint, llPoint);
			Handles.DrawLine(llPoint, ulPoint);
		}

		private void ShowPoint(BoxCorner corner)
		{
			Vector3 point = area.GetCornerPosition(corner);

			EditorGUI.BeginChangeCheck();
			point = Handles.FreeMoveHandle(point, Quaternion.identity, 0.1f, Vector3.one, Handles.DotHandleCap);
			if (EditorGUI.EndChangeCheck())
			{
				area.SetCornerPosition(corner, point);
				changing = true;
			}
			else if (changing)
			{
				Undo.RecordObject(area, "Change area");
				EditorUtility.SetDirty(area);
				area.SetCornerPosition(corner, point);
				changing = false;
			}
		}
	}
}

