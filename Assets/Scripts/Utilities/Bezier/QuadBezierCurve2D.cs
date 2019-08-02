using System;
using UnityEngine;

namespace JTUtility.Bezier
{
    [Serializable]
    public class QuadBezierCurve2D
    {
        public Transform parent;
        [SerializeField] protected Vector2 tailPoint;
        [SerializeField] protected Vector2 controlPoint;
        [SerializeField] protected Vector2 headPoint;

        public Vector2 TailPoint
        {
            get { return parent.TransformPoint(tailPoint); }
            set { tailPoint = parent.InverseTransformPoint(value); }
        }

        public Vector2 HeadPoint
        {
            get { return parent.TransformPoint(headPoint); }
            set { headPoint = parent.InverseTransformPoint(value); }
        }

        public Vector2 ControlPoint
        {
            get { return parent.TransformPoint(controlPoint); }
            set { controlPoint = parent.InverseTransformPoint(value); }
        }

        public Vector2 RawTailPoint
        {
            get { return tailPoint; }
            set { tailPoint = value; }
        }

        public Vector2 RawHeadPoint
        {
            get { return headPoint; }
            set { headPoint = value; }
        }

        public Vector2 RawControlPoint
        {
            get { return controlPoint; }
            set { controlPoint = value; }
        }

        public QuadBezierCurve2D(Transform parent)
        {
            this.parent = parent;
            tailPoint = new Vector2(0, 0);
            controlPoint = new Vector2(1, 0);
            headPoint = new Vector2(2, 0);
        }

        public QuadBezierCurve2D(Transform parent, Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint)
        {
            this.parent = parent;
            this.tailPoint = startPoint;
            this.controlPoint = controlPoint;
            this.headPoint = endPoint;
        }

        public Vector2 GetPoint(float t)
        {
            return parent.TransformPoint(Bezier.GetPoint(tailPoint, controlPoint, headPoint, t));
        }

        public Vector2 GetVelocity(float t)
        {
            return parent.TransformPoint(Bezier.GetFirstDerivative(tailPoint, controlPoint, headPoint, t)) - parent.position;
        }

        public Vector2 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public Vector2 GetClosestPoint(Vector2 point, out float t)
        {
            point = parent.InverseTransformPoint(point);
            return parent.TransformPoint(Bezier.GetClosestPoint(tailPoint, controlPoint, headPoint, point, out t));
        }
    }
}
