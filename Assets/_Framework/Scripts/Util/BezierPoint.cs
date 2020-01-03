using System;
using UnityEngine;
using _Framework.Scripts.Extensions;

namespace _Framework.Scripts.Util
{
    [Serializable]
    public struct BezierPoint
    {
        /// <summary>
        /// 	- Enumeration describing the relationship between a point's handles
        /// 	- Connected : The point's handles are mirrored across the point
        /// 	- Broken : Each handle moves independently of the other
        /// 	- None : This point has no handles (both handles are located ON the point)
        /// </summary>
        public enum HandleTypeEnum
        {
            Connected,
            Broken,
            None,
        }

        public Vector3 Position;
        public Vector3 Handle1;
        public Vector3 Handle2;
        public float NormalRotation;
        public HandleTypeEnum HandleType;

        public static BezierPoint operator *(Vector3 v, BezierPoint b)
        {
            return new BezierPoint()
            {
                Position = b.Position.Multiply(v),
                Handle1 = b.Handle1.Multiply(v),
                Handle2 = b.Handle2.Multiply(v)
            };
        }

        public static BezierPoint operator *(BezierPoint b, Vector3 v)
        {
            return new BezierPoint()
            {
                Position = b.Position.Multiply(v),
                Handle1 = b.Handle1.Multiply(v),
                Handle2 = b.Handle2.Multiply(v)
            };
        }

        public static BezierPoint operator *(float v, BezierPoint b)
        {
            return new BezierPoint()
            {
                Position = b.Position.Multiply(v),
                Handle1 = b.Handle1.Multiply(v),
                Handle2 = b.Handle2.Multiply(v)
            };
        }

        public static BezierPoint operator *(BezierPoint b, float v)
        {
            return new BezierPoint()
            {
                Position = b.Position.Multiply(v),
                Handle1 = b.Handle1.Multiply(v),
                Handle2 = b.Handle2.Multiply(v)
            };
        }

        public static bool operator ==(BezierPoint c1, BezierPoint c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(BezierPoint c1, BezierPoint c2)
        {
            return !c1.Equals(c2);
        }
    }
//
//    public static class BezierPointExtensions
//    {
//
//        public static float Evaluate(this BezierPoint[] points, float t, int resolution = 30)
//        {
//            var approxPoints = points.ApproximatePoints(resolution);
//            var x = approxPoints.Max(vector3 => vector3.x) * t;
//            var maxy = approxPoints.Max(vector3 => vector3.y);
//
//            if(t <= 0) return approxPoints[0].y / maxy;
//            if(t >= 1) return approxPoints[approxPoints.Length - 1].y / maxy;
//
//            for (var index = 0; index < approxPoints.Length - 1; index++)
//            {
//                var cur = approxPoints[index];
//                var next = approxPoints[index + 1];
//
//                if (cur.x <= x && next.x >= x)
//                {
//                    var vec = next - cur;
//                    var percVec = (x - cur.x) / (next.x - cur.x);
//                    return (cur + vec * percVec).y / maxy;
//                }
//            }
//
//            throw new InvalidOperationException("Error in algorithm.");
//        }
//
//        public static Vector3 Evaluate2(this BezierPoint[] points, float t)
//        {
////            var maxy = points.ApproximatePoints(10).Max(vector3 => vector3.y);
////            return points.GetPointAt(t).y / maxy;
//            return points.GetPointAt(t);
//        }
//        /// <summary>
//        /// 	- Gets the point at 't' percent along this curve
//        /// </summary>
//        /// <returns>
//        /// 	- Returns the point at 't' percent
//        /// </returns>
//        /// <param name='t'>
//        /// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
//        /// </param>
//        public static Vector3 GetPointAt(this BezierPoint[] points, float t)
//        {
//            if (t <= 0) return points[0].Position;
//            else if (t >= 1) return points[points.Length - 1].Position;
//
//            float totalPercent = 0;
//            float curvePercent = 0;
//
//            BezierPoint p1 = new BezierPoint();
//            BezierPoint p2 = new BezierPoint();
//
//            var length = Length(points);
//            for (int i = 0; i < points.Length - 1; i++)
//            {
//                curvePercent = ApproximateLength(points[i], points[i + 1], 10) / length;
//                if (totalPercent + curvePercent > t)
//                {
//                    p1 = points[i];
//                    p2 = points[i + 1];
//                    break;
//                }
//
//                else totalPercent += curvePercent;
//            }
//
//            t -= totalPercent;
//
//            return GetPoint(p1, p2, t / curvePercent);
//        }
//
//        public static float Length(this BezierPoint[] points, int resolution = 30)
//        {
//            var _length = 0f;
//            for (int i = 0; i < points.Length - 1; i++)
//            {
//                _length += ApproximateLength(points[i], points[i + 1], resolution);
//            }
//
//            return _length;
//        }
//
//        public static float ApproximateLength(BezierPoint p1, BezierPoint p2, int resolution = 30)
//        {
//            float _res = resolution;
//            float total = 0;
//            Vector3 lastPosition = p1.Position;
//            Vector3 currentPosition;
//
//            for (int i = 0; i < resolution + 1; i++)
//            {
//                currentPosition = GetPoint(p1, p2, i / _res);
//                total += (currentPosition - lastPosition).magnitude;
//                lastPosition = currentPosition;
//            }
//
//            return total;
//        }
//
//        public static Vector3 GetPoint(float t, params Vector3[] points)
//        {
//            t = Mathf.Clamp01(t);
//
//            int order = points.Length - 1;
//            Vector3 point = Vector3.zero;
//            Vector3 vectorToAdd;
//
//            for (int i = 0; i < points.Length; i++)
//            {
//                vectorToAdd = points[points.Length - i - 1] * (BinomialCoefficient(i, order) * Mathf.Pow(t, order - i) * Mathf.Pow((1 - t), i));
//                point += vectorToAdd;
//            }
//
//            return point;
//        }
//
//        private static int BinomialCoefficient(int i, int n)
//        {
//            return Factoral(n) / (Factoral(i) * Factoral(n - i));
//        }
//
//        private static int Factoral(int i)
//        {
//            if (i == 0) return 1;
//
//            int total = 1;
//
//            while (i - 1 >= 0)
//            {
//                total *= i;
//                i--;
//            }
//
//            return total;
//        }
//
//        public static Vector3[] ApproximatePoints(this BezierPoint[] points, int resolution = 30)
//        {
//            var output = new List<Vector3>();
//            for (var i = 0; i < points.Length - 1; i++)
//            {
//                output.AddRange(ApproxCurve(points[i], points[i + 1], resolution));
//            }
//
//            return output.ToArray();
//        }
//
//        private static IEnumerable<Vector3> ApproxCurve(BezierPoint pointA, BezierPoint pointB, int res)
//        {
//            return Enumerable.Range(0, res + 1)
//                .Select(i => GetPoint(pointA, pointB, (float)i / res))
//                .ToList();
//        }
//
//        private static Vector3 GetPoint(BezierPoint p1, BezierPoint p2, float t)
//        {
//            if (p1.Handle2 != Vector3.zero)
//            {
//                if (p2.Handle1 != Vector3.zero)
//                {
//                    return GetCubicCurvePoint(p1.Position, p1.Position + p1.Handle2, p2.Position + p2.Handle1, p2.Position, t);
//                }
//
//                return GetQuadraticCurvePoint(p1.Position, p1.Position + p1.Handle2, p2.Position, t);
//            }
//
//            if (p2.Handle1 != Vector3.zero)
//            {
//                return GetQuadraticCurvePoint(p1.Position, p2.Position + p2.Handle1, p2.Position, t);
//            }
//
//            return GetLinearPoint(p1.Position, p2.Position, t);
//        }
//
//        private static Vector3 GetCubicCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
//        {
//            t = Mathf.Clamp01(t);
//
//            var part1 = Mathf.Pow(1 - t, 3) * p1;
//            var part2 = 3 * Mathf.Pow(1 - t, 2) * t * p2;
//            var part3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p3;
//            var part4 = Mathf.Pow(t, 3) * p4;
//
//            return part1 + part2 + part3 + part4;
//        }
//
//
//        private static Vector3 GetQuadraticCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
//        {
//            t = Mathf.Clamp01(t);
//
//            var part1 = Mathf.Pow(1 - t, 2) * p1;
//            var part2 = 2 * (1 - t) * t * p2;
//            var part3 = Mathf.Pow(t, 2) * p3;
//
//            return part1 + part2 + part3;
//        }
//
//        private static Vector3 GetLinearPoint(Vector3 p1, Vector3 p2, float t)
//        {
//            return p1 + ((p2 - p1) * t);
//        }
//    }
}