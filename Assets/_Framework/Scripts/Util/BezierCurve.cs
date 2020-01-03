#region UsingStatements

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Framework.Scripts.Extensions;

#endregion

namespace _Framework.Scripts.Util
{
    /// <summary>
    /// 	- Class for describing and drawing Bezier Curves
    /// 	- Efficiently handles approximate length calculation through 'dirty' system
    /// 	- Has static functions for getting points on curves constructed by Vector3 parameters (GetPoint, GetCubicPoint, GetQuadraticPoint, and GetLinearPoint)
    /// </summary>
    [Serializable]
    public class BezierCurve
    {

        #region PublicVariables

        /// <summary>
        ///  	- the number of mid-points calculated for each pair of bezier points
        ///  	- used for drawing the curve in the editor
        ///  	- used for calculating the "length" variable
        /// </summary>
        public int resolution = 30;

        #endregion

        #region PublicProperties

        /// <summary>
        ///		- set in the editor
        /// 	- used to determine if the curve should be drawn as "closed" in the editor
        /// 	- used to determine if the curve's length should include the curve between the first and the last points in "points" array
        /// 	- setting this value will cause the curve to become dirty
        /// </summary>
        [SerializeField] private bool _close;
        public bool close
        {
            get { return _close; }
            set
            {
                if (_close == value) return;
                _close = value;

                Recalculate();
            }
        }

        private void Recalculate()
        {
            length = RecalculateLength();

            var points = ApproximatePoints();
            if (points.Any())
            {
                var temp = new Bounds(points.First(), Vector3.zero);
                points.ForEach(p => temp.Encapsulate(p));
                temp.size = new Vector3(
                    temp.size.x == 0 ? 1 : temp.size.x,
                    temp.size.y == 0 ? 1 : temp.size.y,
                    temp.size.z == 0 ? 1 : temp.size.z
                    );
                bounds = temp;
            }
            else
            {
                bounds = new Bounds();
            }
        }

        private float RecalculateLength()
        {
            var length = 0f;
            for (int i = 0; i < points.Length - 1; i++)
            {
                length += ApproximateLength(points[i], points[i + 1], resolution);
            }

            if (close) length += ApproximateLength(points[points.Length - 1], points[0], resolution);

            return length;
        }

        /// <summary>
        ///		- set internally
        ///		- gets point corresponding to "index" in "points" array
        ///		- does not allow direct set
        /// </summary>
        /// <param name='index'>
        /// 	- the index
        /// </param>
        public BezierPoint this[int index]
        {
            get { return points[index]; }
            set
            {
                points[index] = value;
                Recalculate();
            }
        }

        /// <summary>
        /// 	- number of points stored in 'points' variable
        ///		- set internally
        ///		- does not include "handles"
        /// </summary>
        /// <value>
        /// 	- The point count
        /// </value>
        public int pointCount => points.Length;

        /// <summary>
        /// 	- The approximate length of the curve
        /// 	- recalculates if the curve is "dirty"
        /// </summary>
        public float length { get; private set; }

        /// <summary>
        /// 	- 3D bounds of the curve
        /// </summary>
        public Bounds bounds { get; private set; } = new Bounds();

        /// <summary> 
        /// 	- Array of point objects that make up this curve
        ///		- Populated through editor
        /// </summary>
        public BezierPoint[] Points
        {
            get { return points.ToArray(); }
            set
            {
                points = value.ToArray();
                Recalculate();
            }
        }

        #endregion

        #region PrivateVariables

        /// <summary> 
        /// 	- Array of point objects that make up this curve
        ///		- Populated through editor
        /// </summary>
        [SerializeField] private BezierPoint[] points;

        public BezierCurve(BezierPoint[] points)
        {
            Points = points;
        }

        #endregion

        #region PublicFunctions

        /// <summary>
        /// 	- Gets the point at 't' percent along this curve
        /// </summary>
        /// <returns>
        /// 	- Returns the point at 't' percent
        /// </returns>
        /// <param name='t'>
        /// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
        /// </param>
        public Vector3 GetPointAt(float t, bool normalize = false)
        {
            if (length == 0) t = 0;
            if (t <= 0) return Normalize(points[0].Position, normalize);
            if (t >= 1) return Normalize(points[close ? 0 : points.Length - 1].Position, normalize);

            float totalPercent = 0;
            float curvePercent = 0;

            BezierPoint? p1 = null;
            BezierPoint? p2 = null;

            var extra = close ? 1 : 0;
            for (int i = 0; i < points.Length - 1 + extra; i++)
            {
                var index0 = i % points.Length;
                var index1 = (i + 1)% points.Length;
                curvePercent = ApproximateLength(points[index0], points[index1], 30) / length;
                if (totalPercent + curvePercent > t)
                {
                    p1 = points[index0];
                    p2 = points[index1];
                    break;
                }

                else totalPercent += curvePercent;
            }
            
            t -= totalPercent;

            var point = GetPoint(p1.Value, p2.Value, t / curvePercent);
            return Normalize(point, normalize);
        }

        /// <summary>
        /// 	- Gets the point at 't' percent along this curve
        /// </summary>
        /// <returns>
        /// 	- Returns the point at 't' percent
        /// </returns>
        /// <param name='t'>
        /// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
        /// </param>
        public float GetRotationAt(float t)
        {
            if (t <= 0) return points[0].NormalRotation;
            if (t >= 1) return points[points.Length - 1].NormalRotation;

            float totalPercent = 0;
            float curvePercent = 0;

            BezierPoint? p1 = null;
            BezierPoint? p2 = null;

            for (int i = 0; i < points.Length - 1; i++)
            {
                curvePercent = ApproximateLength(points[i], points[i + 1], 30) / length;
                if (totalPercent + curvePercent > t)
                {
                    p1 = points[i];
                    p2 = points[i + 1];
                    break;
                }

                else totalPercent += curvePercent;
            }

            if (close || p1.HasValue == false)
            {
                p1 = points[points.Length - 1];
                p2 = points[0];
            }

            t -= totalPercent;

            var f = t / curvePercent;
            return Mathf.Lerp(p1.Value.NormalRotation, p2.Value.NormalRotation, f);
        }

        private Vector3 Normalize(Vector3 point, bool normalize)
        {
            if (normalize == false) return point;
            
            point.x /= bounds.size.x;
            point.y /= bounds.size.y;
            point.z /= bounds.size.z;
            return point;
        }

        /// <summary>
        /// 	- Get the index of the given point in this curve
        /// </summary>
        /// <returns>
        /// 	- The index, or -1 if the point is not found
        /// </returns>
        /// <param name='point'>
        /// 	- Point to search for
        /// </param>
        public int GetPointIndex(BezierPoint point)
        {
            int result = -1;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == point)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        #endregion

        #region PublicStaticFunctions

        public float Evaluate(float t, int resolution = 30)
        {
            var approxPoints = ApproximatePoints(resolution);

            if (t <= 0) return approxPoints[0].y / bounds.size.y;
            if (t >= 1) return approxPoints[approxPoints.Length - 1].y / bounds.size.y;

            for (var index = 0; index < approxPoints.Length - 1; index++)
            {
                var cur = approxPoints[index];
                var next = approxPoints[index + 1];

                var x = bounds.size.x * t;
                if (cur.x <= x && next.x >= x)
                {
                    var vec = next - cur;
                    var percVec = (x - cur.x) / (next.x - cur.x);
                    return (cur + vec * percVec).y / bounds.size.y;
                }
            }

            throw new InvalidOperationException("Error in algorithm.");
        }

        public Vector3[] ApproximatePoints(int resolution = 30, bool clean = false)
        {
            var output = new List<Vector3>();

            var length = points.Length  + (close ? 1 : 0);
            for (var i = 0; i < length - 1; i++)
            {
                var cur = points[i];
                var next = points[(i + 1) % points.Length];

                var keepLast = close == false && i == length - 2;
                var approxCurve = ApproxCurve(cur, next, resolution);
                if (keepLast)
                {
                    output.AddRange(approxCurve);
                }
                else
                {
                    output.AddRange(approxCurve.Take(approxCurve.Count - 1));
                }
                
            }

            if (clean)
            {
                output = CleanPoints(output);
            }
            
            return output.ToArray();
        }
        

        private List<Vector3> CleanPoints(List<Vector3> points)
        {
            if (points.Count <= 2) return points;

            var path = new List<Vector3>();
            path.Add(points.First());

            for (var i = 1; i < points.Count - 1; i++)
            {
                var prevDir = (points[i] - points[i - 1]).normalized;
                var curDir = (points[i + 1] - points[i]).normalized;

                if (prevDir != curDir)
                {
                    path.Add(points[i]);
                }
            }

            path.Add(points.Last());

            return path;
        }

        private List<Vector3> ApproxCurve(BezierPoint pointA, BezierPoint pointB, int res)
        {
            return Enumerable.Range(0, res + 1)
                .Select(i => GetPoint(pointA, pointB, (float)i / res))
                .ToList();
        }

        public static Vector3 GetPoint(BezierPoint p1, BezierPoint p2, float t)
        {
            if (p1.Handle2 != Vector3.zero)
            {
                if (p2.Handle1 != Vector3.zero)
                {
                    return GetCubicCurvePoint(p1.Position, p1.Position + p1.Handle2, p2.Position + p2.Handle1, p2.Position, t);
                }

                return GetQuadraticCurvePoint(p1.Position, p1.Position + p1.Handle2, p2.Position, t);
            }

            if (p2.Handle1 != Vector3.zero)
            {
                return GetQuadraticCurvePoint(p1.Position, p2.Position + p2.Handle1, p2.Position, t);
            }

            return GetLinearPoint(p1.Position, p2.Position, t);
        }

        private static Vector3 GetCubicCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            t = Mathf.Clamp01(t);

            var part1 = Mathf.Pow(1 - t, 3) * p1;
            var part2 = 3 * Mathf.Pow(1 - t, 2) * t * p2;
            var part3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p3;
            var part4 = Mathf.Pow(t, 3) * p4;

            return part1 + part2 + part3 + part4;
        }


        private static Vector3 GetQuadraticCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);

            var part1 = Mathf.Pow(1 - t, 2) * p1;
            var part2 = 2 * (1 - t) * t * p2;
            var part3 = Mathf.Pow(t, 2) * p3;

            return part1 + part2 + part3;
        }

        private static Vector3 GetLinearPoint(Vector3 p1, Vector3 p2, float t)
        {
            return p1 + ((p2 - p1) * t);
        }

        /// <summary>
        /// 	- Gets point 't' percent along n-order curve
        /// </summary>
        /// <returns>
        /// 	- The point 't' percent along the curve
        /// </returns>
        /// <param name='t'>
        /// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
        /// </param>
        /// <param name='points'>
        /// 	- The points used to define the curve
        /// </param>
        public Vector3 GetPoint(float t, params Vector3[] points)
        {
            t = Mathf.Clamp01(t);

            int order = points.Length - 1;
            Vector3 point = Vector3.zero;
            Vector3 vectorToAdd;

            for (int i = 0; i < points.Length; i++)
            {
                vectorToAdd = points[points.Length - i - 1] * (BinomialCoefficient(i, order) * Mathf.Pow(t, order - i) * Mathf.Pow((1 - t), i));
                point += vectorToAdd;
            }

            return point;
        }

        /// <summary>
        /// 	- Approximates the length
        /// </summary>
        /// <returns>
        /// 	- The approximate length
        /// </returns>
        /// <param name='p1'>
        /// 	- The bezier point at the start of the curve
        /// </param>
        /// <param name='p2'>
        /// 	- The bezier point at the end of the curve
        /// </param>
        /// <param name='resolution'>
        /// 	- The number of points along the curve used to create measurable segments
        /// </param>
        public static float ApproximateLength(BezierPoint p1, BezierPoint p2, int resolution = 10)
        {
            float _res = resolution;
            float total = 0;
            Vector3 lastPosition = p1.Position;
            Vector3 currentPosition;

            for (int i = 0; i < resolution + 1; i++)
            {
                currentPosition = GetPoint(p1, p2, i / _res);
                total += (currentPosition - lastPosition).magnitude;
                lastPosition = currentPosition;
            }

            return total;
        }

        #endregion

        #region UtilityFunctions

        private static int BinomialCoefficient(int i, int n)
        {
            return Factoral(n) / (Factoral(i) * Factoral(n - i));
        }

        private static int Factoral(int i)
        {
            if (i == 0) return 1;

            int total = 1;

            while (i - 1 >= 0)
            {
                total *= i;
                i--;
            }

            return total;
        }

        #endregion

        /* needs testing
	public Vector3 GetPointAtDistance(float distance)
	{
		if(close)
		{
			if(distance < 0) while(distance < 0) { distance += length; }
			else if(distance > length) while(distance > length) { distance -= length; }
		}
		
		else
		{
			if(distance <= 0) return points[0].position;
			else if(distance >= length) return points[points.Length - 1].position;
		}
		
		float totalLength = 0;
		float curveLength = 0;
		
		BezierPoint firstPoint = null;
		BezierPoint secondPoint = null;
		
		for(int i = 0; i < points.Length - 1; i++)
		{
			curveLength = ApproximateLength(points[i], points[i + 1], resolution);
			if(totalLength + curveLength >= distance)
			{
				firstPoint = points[i];
				secondPoint = points[i+1];
				break;
			}
			else totalLength += curveLength;
		}
		
		if(firstPoint == null)
		{
			firstPoint = points[points.Length - 1];
			secondPoint = points[0];
			curveLength = ApproximateLength(firstPoint, secondPoint, resolution);
		}
		
		distance -= totalLength;
		return GetPoint(distance / curveLength, firstPoint, secondPoint);
	}
	*/

        protected bool Equals(BezierCurve other)
        {
            return _close == other._close && points.SequenceEqual(other.points);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BezierCurve) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_close.GetHashCode() * 397) ^ (points != null ? points.GetHashCode() : 0);
            }
        }

        public static bool operator ==(BezierCurve left, BezierCurve right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BezierCurve left, BezierCurve right)
        {
            return !Equals(left, right);
        }
    }
}