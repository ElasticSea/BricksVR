using System;
using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts
{
    public class Block : MonoBehaviour
    {
        private BlockClone blockClone;
        private bool snapActive;

        private (Transform thisSocket, Transform otherSocket)[] GetConnections()
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
            blockClone = BlockClone.SetupBlock(this);
        }
        
        private void FixedUpdate()
        {
            var connections = GetConnections();

            if (snapActive && connections.Length >= 2)
            {
                blockClone.gameObject.SetActive(true);
                
                // Chose two closes connections and choose origin and alignment.
                var thisSocketA = connections[0].thisSocket;
                var otherSocketA = connections[0].otherSocket;
                var thisSocketB = connections[1].thisSocket;
                var otherSocketB = connections[1].otherSocket;

                Snap(thisSocketA, thisSocketB, otherSocketA, otherSocketB);
            }
            else
            {
                blockClone.gameObject.SetActive(false);
            }
        }

        private void Snap(Transform thisA, Transform thisB, Transform otherA, Transform otherB)
        {
            var thisDir = (thisB.position - thisA.position).normalized;
            var otherDir = (otherB.position - otherA.position).normalized;

            var thisToOtherRotation = Quaternion.FromToRotation(thisDir, otherDir);

            // Correct for rotation along the direction (multiple valid states for resulting rotation)
            var correctedUpVector = thisToOtherRotation * -thisA.up;
            var angle = Vector3.SignedAngle(correctedUpVector, otherA.up, otherDir);
            var correction = Quaternion.AngleAxis(angle, otherDir);

            var targetRotation = correction * thisToOtherRotation * transform.rotation;

            blockClone.transform.rotation = targetRotation;
            
            // TODO Get the resulting position and rotation without touching the transforms
            // Adjust position
            blockClone.transform.position = transform.position;
            var blockSocketLocalPosition = transform.InverseTransformPoint(thisA.position);
            var adjustedWorldPosition = blockClone.transform.TransformPoint(blockSocketLocalPosition);
            var offset = otherA.position - adjustedWorldPosition;
            blockClone.transform.position += offset;
        }

        public void BeginSnap()
        {
            snapActive = true;
        }

        public void EndSnap()
        {
            var result = blockClone.Locked;

            if (result.Any())
            {
                ConnectBlocks(result);
            }

            snapActive = false;
        }

        private void ConnectBlocks((Socket ThisSocket, Socket OtherSocket)[] result)
        {
            // Align this block
            transform.CopyWorldFrom(blockClone.transform);
            
            var allBlocks = result
                .SelectMany(tuple => new[] {tuple.ThisSocket, tuple.OtherSocket})
                .Select(socket => socket.GetComponentInParent<Block>().gameObject)
                .Distinct()
                .ToList();

            foreach (var (thisSocket, otherSocket) in result)
            {
                var thisLink = thisSocket.GetComponentInParent<BlockLink>();
                var otherLink = otherSocket.GetComponentInParent<BlockLink>();

                // Just connnect the links
                thisLink.Connect(otherLink);
            }

            DestroyImmediate(blockClone.gameObject);
            foreach (var block in allBlocks)
            {
                DestroyImmediate(block.GetComponent<Rigidbody>());
                DestroyImmediate(block.GetComponent<Block>());
            }
            
            foreach (var block in allBlocks.Skip(1))
            {
                foreach (var child in block.transform.Children())
                {
                    child.transform.SetParent(allBlocks[0].transform, true);
                }
                DestroyImmediate(block);
            }

            allBlocks[0].gameObject.AddComponent<Rigidbody>();
            var blk = allBlocks[0].gameObject.AddComponent<Block>();
            var blockGrab = allBlocks[0].gameObject.GetComponent<BlockGrab>();
            blockGrab.SetupGrabPoints(allBlocks[0].GetComponentsInChildren<Collider>());
            blockGrab.Block = blk;
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

            foreach (var (a, b) in GetComponentInChildren<BlockLink>().GetAllEdges())
            {
                Gizmos.color = Color.yellow;

                var from = a.transform.TransformPoint(a.gameObject.GetLocalCompositeMeshBounds().Value.center);
                var to = b.transform.TransformPoint(b.gameObject.GetLocalCompositeMeshBounds().Value.center);
                Gizmos.DrawLine(from, to);
            }
        }
    }
}