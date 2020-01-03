using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Framework.Scripts.Util;

namespace _Framework.Scripts.Util
{
    public class BezierCurveVisualTest : MonoBehaviour
    {
        [SerializeField] private BezierCurve curve = new BezierCurve(new[]
        {
            new BezierPoint
            {
                Position = new Vector3(0, .5f, 0),
                Handle1 = new Vector3(-0.275957512247f, 0, 0f),
                Handle2 = new Vector3(+0.275957512247f, 0, 0f),
                HandleType = BezierPoint.HandleTypeEnum.Connected
            },
            new BezierPoint
            {
                Position = new Vector3(.5f, 0, 0),
                Handle1 = new Vector3(0, +0.275957512247f, 0),
                Handle2 = new Vector3(0, -0.275957512247f, 0),
                HandleType = BezierPoint.HandleTypeEnum.Connected
            },
            new BezierPoint
            {
                Position = new Vector3(0, -.5f, 0),
                Handle1 = new Vector3(+0.275957512247f, 0, 0f),
                Handle2 = new Vector3(-0.275957512247f, 0, 0f),
                HandleType = BezierPoint.HandleTypeEnum.Connected
            },
            new BezierPoint
            {
                Position = new Vector3(-.5f, 0, 0),
                Handle1 = new Vector3(0, -0.275957512247f, 0),
                Handle2 = new Vector3(0, +0.275957512247f, 0),
                HandleType = BezierPoint.HandleTypeEnum.Connected
            }
        });

        [SerializeField] private int count = 100;
        [SerializeField] private float sphereSize = 0.01f;

        private void OnDrawGizmos()
        {
            curve.Points = curve.Points;
            var points = Enumerable.Range(0, count).Select(i => curve.GetPointAt(i / (count - 1f))).ToArray();

            for (int i = 0; i <= points.Length; i++)
            {
                var index0 = i % points.Length;
                var first = points[index0];
                var index1 = (i + 1) % points.Length;

                if (index0 < index1 || curve.close)
                {
                    var second = points[index1];

                    Gizmos.color = Color.Lerp(Color.blue, Color.red, i / (count - 1f));
                    Gizmos.DrawSphere(first, sphereSize);
                    Gizmos.DrawSphere(second, sphereSize);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(first, second);
                }
            }
        }
    }
}