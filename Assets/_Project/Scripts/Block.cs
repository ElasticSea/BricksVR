using System;
using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private SnapPreview snapPreview;

        public (Transform thisSocket, Transform otherSocket)[] GetConnections()
        {
            var sockets = transform.GetComponentsInChildren<Socket>();
            var connected = new Socket[sockets.Length];
            
            // Find all connected pairs
            for (var i = 0; i < sockets.Length; i++)
            {
                var socket = sockets[i];
                var candidate = socket.GetComponent<Socket>().Trigger();
                if (candidate.IsEmpty() == false)
                {
                    var closest = candidate.OrderBy(o => o.transform.position.Distance(socket.transform.position)).First();
                    var sockets2 = closest;
                    if (sockets2.Type != socket.Type)
                    {
                        connected[i] = closest;
                    }
                }
            }

            // Sort connection for the closest one
            return sockets.Select((socket, i) =>
                {
                    if (connected[i] == null) return null;
                    var dist = sockets[i].transform.position.Distance(connected[i].transform.position);
                    return new {Index = i, Distance = dist};
                })
                .Where(it => it != null)
                .OrderBy(arg => arg.Distance)
                .Select(arg => (sockets[arg.Index].transform, connected[arg.Index].transform))
                .ToArray();
        }

        private void Start()
        {
            snapPreview = SnapPreview.CreateFromBlock(this);
        }

        public void BeginSnap()
        {
            snapPreview.BeginSnap();
        }

        public void EndSnap()
        {
            snapPreview.EndSnap();
        }

        private void OnDrawGizmosSelected()
        {
            var connections = GetConnections();
            for (var i = 0; i < connections.Length; i++)
            {
                var color = Color.white;
                var size = 0.01f;
                if (i == 0) color = Color.red;
                if (i == 1) color = Color.blue;
                if (i >= 2) size = 0.005f;
                
                var from = connections[i].thisSocket.transform.position;
                var to = connections[i].otherSocket.transform.position;
                
                Gizmos.color = color.SetAlpha(.5f);
                Gizmos.DrawSphere(from, size);
                Gizmos.DrawSphere(to, size);
                Gizmos.DrawLine(from, to);
            }

            foreach (var (a, b) in GetComponentInChildren<ChunkLink>().GetAllEdges())
            {
                Gizmos.color = Color.yellow;

                var from = a.transform.TransformPoint(a.gameObject.GetLocalCompositeMeshBounds().Value.center);
                var to = b.transform.TransformPoint(b.gameObject.GetLocalCompositeMeshBounds().Value.center);
                Gizmos.DrawLine(from, to);
                Gizmos.DrawSphere(from, .0025f);
                Gizmos.DrawSphere(to, .0025f);
            }
        }

        private void OnDestroy()
        {
            DestroyImmediate(snapPreview.gameObject);
        }
    }
}