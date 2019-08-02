using UnityEngine;

namespace JTUtility.Bezier
{
    public static class Bezier
    {
        #region 2D
        public static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }

        // http://blog.gludion.com/2009/08/distance-to-quadratic-bezier-curve.html
        public static Vector2 GetClosestPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 point)
        {
            var toPoint = p0 - point;
            var A = p1 - p0;
            var B = p2 - p1 - A;
            var a = Vector2.Dot(B, B);
            var b = 3 * Vector2.Dot(A, B);
            var c = 2 * Vector2.Dot(A, A) + Vector2.Dot(toPoint, B);
            var d = Vector2.Dot(toPoint, A);
            double[] results;

            Maths.SolveCubicEquation(a, b, c, d, out results);

            Vector2 closestPoint = Vector2.zero;
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < results.Length; i++)
            {
                if (double.IsNaN(results[i])) break;

                var cPoint = GetPoint(p0, p1, p2, (float)results[i]);
                var dist = (cPoint - point).sqrMagnitude;
                if (dist < minDist)
                {
                    closestPoint = cPoint;
                    minDist = dist;
                }
            }

            return closestPoint;
        }

        // http://blog.gludion.com/2009/08/distance-to-quadratic-bezier-curve.html
        public static Vector2 GetClosestPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 point, out float t)
        {
            var toPoint = p0 - point;
            var A = p1 - p0;
            var B = p2 - p1 - A;
            var a = Vector2.Dot(B, B);
            var b = 3 * Vector2.Dot(A, B);
            var c = 2 * Vector2.Dot(A, A) + Vector2.Dot(toPoint, B);
            var d = Vector2.Dot(toPoint, A);
            double[] results;

            Maths.SolveCubicEquation(a, b, c, d, out results);

            t = 0;
            Vector2 closestPoint = Vector2.zero;
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < results.Length; i++)
            {
                if (double.IsNaN(results[i])) break;

                var cPoint = GetPoint(p0, p1, p2, (float)results[i]);
                var dist = (cPoint - point).sqrMagnitude;
                if (dist < minDist)
                {
                    t = (float)results[i];
                    closestPoint = cPoint;
                    minDist = dist;
                }
            }

            return closestPoint;
        }

        public static Vector2 GetFirstDerivative(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            return
                2f * (1f - t) * (p1 - p0) +
                2f * t * (p2 - p1);
        }

        public static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float OneMinusT = 1f - t;
            return
                OneMinusT * OneMinusT * OneMinusT * p0 +
                3f * OneMinusT * OneMinusT * t * p1 +
                3f * OneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public static Vector2 GetFirstDerivative(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
        #endregion

        #region 3D
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return
                2f * (1f - t) * (p1 - p0) +
                2f * t * (p2 - p1);
        }

        public static Vector3 GetClosestPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 point)
        {
            var toPoint = p0 - point;
            var A = p1 - p0;
            var B = p2 - p1 - A;
            var a = Vector3.Dot(B, B);
            var b = 3 * Vector3.Dot(A, B);
            var c = 2 * Vector3.Dot(A, A) + Vector3.Dot(toPoint, B);
            var d = Vector3.Dot(toPoint, A);
            float[] results;

            Maths.SolveCubicEquation(a, b, c, d, out results);

            Vector3 closestPoint = Vector3.zero;
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < results.Length; i++)
            {
                if (float.IsNaN(results[i])) break;

                var cPoint = GetPoint(p0, p1, p2, results[i]);
                var dist = (cPoint - point).sqrMagnitude;
                if (dist < minDist)
                {
                    closestPoint = cPoint;
                    minDist = dist;
                }
            }

            return closestPoint;
        }

        public static Vector3 GetClosestPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 point, out float t)
        {
            var toPoint = p0 - point;
            var A = p1 - p0;
            var B = p2 - p1 - A;
            var a = Vector3.Dot(B, B);
            var b = 3 * Vector3.Dot(A, B);
            var c = 2 * Vector3.Dot(A, A) + Vector3.Dot(toPoint, B);
            var d = Vector3.Dot(toPoint, A);
            float[] results;

            Maths.SolveCubicEquation(a, b, c, d, out results);

            t = 0;
            Vector3 closestPoint = Vector3.zero;
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < results.Length; i++)
            {
                if (float.IsNaN(results[i])) break;

                var cPoint = GetPoint(p0, p1, p2, results[i]);
                var dist = (cPoint - point).sqrMagnitude;
                if (dist < minDist)
                {
                    t = results[i];
                    closestPoint = cPoint;
                    minDist = dist;
                }
            }

            return closestPoint;
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float OneMinusT = 1f - t;
            return
                OneMinusT * OneMinusT * OneMinusT * p0 +
                3f * OneMinusT * OneMinusT * t * p1 +
                3f * OneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
        #endregion
    }
}
