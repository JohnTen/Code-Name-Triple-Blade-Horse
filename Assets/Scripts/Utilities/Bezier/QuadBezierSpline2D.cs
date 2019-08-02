
/*
namespace UnityUtility.Bezier
{
	[System.Serializable]
	public class QuadBezierSpline2D
	{
		[SerializeField] protected bool loop;

		[SerializeField] protected List<QuadBezierCurve2D> curves = new List<QuadBezierCurve2D>();

		public virtual bool Loop
		{
			get { return loop; }
			set
			{
				loop = value;
				if (value == true)
				{
					LinkCurves(curves.Count - 1, 0);
				}
			}
		}

		public virtual void AddCurve()
		{
			curves.Add(new QuadBezierCurve2D(this.transform));
			LinkCurves(curves.Count - 2, curves.Count - 1);
			if (loop)
				LinkCurves(curves.Count - 1, 0);
			
		}

		public virtual void MoveConnectPoint(int index, Vector2 position)
		{
			if (loop && index == curves.Count) index = 0;

			if (index == curves.Count)
				curves[index-1].HeadPoint = position;
			else
				curves[index].TailPoint = position;
		
			if (index == 0 && loop)
			{
				LinkCurves(curves.Count - 1, 0);
			}
			else if (index < curves.Count && index > 0)
			{
				LinkCurves(index - 1, index);
			}

			if (loop)
			{
				LinkCurves(curves.Count - 1, 0);
			}
			
		}

		public virtual void MoveControlPoint(int index, Vector2 position)
		{
			curves[index].ControlPoint = position;
		}

		public virtual Vector2 FindClosestPoint(Vector2 point)
		{
			float minDist = float.PositiveInfinity;
			Vector3 closestPoint = Vector2.zero;
		
			for (int i = 0; i < curves.Count; i++)
			{
				float t;
				curves[i].GetClosestPoint(point, out t);
				var cPoint = curves[i].GetPoint(t);
				var dist = (cPoint - point).sqrMagnitude;
				if (minDist > dist)
				{
					minDist = dist;
					closestPoint = cPoint;
				}
			}

			return closestPoint;
		}

		public virtual Vector2 FindClosestPoint(Vector2 point, out Vector2 direction)
		{
			float minDist = float.PositiveInfinity;
			Vector3 closestPoint = Vector2.zero;

			direction = Vector2.zero;
			for (int i = 0; i < curves.Count; i++)
			{
				float t;
				var cPoint = curves[i].GetClosestPoint(point, out t);
				var dist = (cPoint - point).sqrMagnitude;

				if (minDist > dist)
				{
					minDist = dist;
					closestPoint = cPoint;

					if (t <= 0)
					{
						var prevIndex = (i - 1 + curves.Count) % curves.Count;
						direction = (curves[prevIndex].GetDirection(1) + curves[i].GetDirection(0)).normalized;
					}
					else if (0 < t && t < 1)
					{
						direction = curves[i].GetDirection(t);
					}
					else if(1 <= t)
					{
						var nextIndex = (i + 1) % curves.Count;
						direction = (curves[nextIndex].GetDirection(0) + curves[i].GetDirection(1)).normalized;
					}
				}
			}

			return closestPoint;
		}

		protected virtual void LinkCurves(int from, int to)
		{
			curves[from].HeadPoint = curves[to].TailPoint;
		}
	}
}
*/
