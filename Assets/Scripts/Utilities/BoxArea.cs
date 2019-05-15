using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTUtility
{
	public enum BoxCorner
	{
		UpperLeft,
		UpperRight,
		LowerLeft,
		LowerRight,
	}

	public class BoxArea : MonoBehaviour
	{
		public Bounds bound = new Bounds();
		public Bounds Bound
		{
			get { return bound; }
			set { bound = value; }
		}

		public Vector3 GetRandomPoint()
		{
			float x = Random.Range(-bound.extents.x, bound.extents.x);
			float y = Random.Range(-bound.extents.y, bound.extents.y);

			return transform.TransformPoint(new Vector3(x, y) + bound.center -transform.position);
		}

		public Vector2 GetCornerPosition(BoxCorner corner)
		{
			Vector2 pos = Vector2.zero;

			switch (corner)
			{
				case BoxCorner.UpperLeft:  pos = bound.center + Vector3.Scale(bound.extents, new Vector3(-1,  1,  0)); break;
				case BoxCorner.UpperRight: pos = bound.center + Vector3.Scale(bound.extents, new Vector3( 1,  1,  0)); break;
				case BoxCorner.LowerLeft:  pos = bound.center + Vector3.Scale(bound.extents, new Vector3(-1, -1,  0)); break;
				case BoxCorner.LowerRight: pos = bound.center + Vector3.Scale(bound.extents, new Vector3( 1, -1,  0)); break;
			}

			return pos;
		}

		public void SetCornerPosition(BoxCorner corner, Vector2 newPosition)
		{
			Vector3 LowerLeft = GetCornerPosition(BoxCorner.UpperLeft);
			Vector3 UpperRight = GetCornerPosition(BoxCorner.LowerRight);
			Vector3 UnchangedPoint = Vector3.zero;

			switch (corner)
			{
				case BoxCorner.UpperLeft:
					UnchangedPoint = GetCornerPosition(BoxCorner.LowerRight);

					LowerLeft = new Vector2(newPosition.x, UnchangedPoint.y);
					UpperRight = new Vector2(UnchangedPoint.x, newPosition.y);
					break;

				case BoxCorner.UpperRight:
					UnchangedPoint = GetCornerPosition(BoxCorner.LowerLeft);

					LowerLeft = UnchangedPoint;
					UpperRight = newPosition;
					break;

				case BoxCorner.LowerLeft:
					UnchangedPoint = GetCornerPosition(BoxCorner.UpperRight);

					LowerLeft = newPosition;
					UpperRight = UnchangedPoint;
					break;

				case BoxCorner.LowerRight:
					UnchangedPoint = GetCornerPosition(BoxCorner.UpperLeft);

					LowerLeft = new Vector2(UnchangedPoint.x, newPosition.y);
					UpperRight = new Vector2(newPosition.x, UnchangedPoint.y);
					break;
			}

			UpperRight.z = transform.position.z;
			LowerLeft.z = transform.position.z;

			bound.SetMinMax(UpperRight, LowerLeft);
		}
	}
}
