using System;
using System.Collections.Generic;
using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts.Blocks
{
    public class SnapPreview : MonoBehaviour
    {
        private BlockGroup blockGroup;
        private Dictionary<Socket, Socket> cloneToBlockMapping;
        public (Socket ThisSocket, Socket OtherSocket)[] Snap { get; private set; }

        private const float LockDistanceEpsilon = 1E-06f;

        private void Update()
        {
            var snapSucessful = TrySnap();
            if (snapSucessful == false)
            {
                Visible = false;
                ColorBlock(false);
                Snap = new (Socket ThisSocket, Socket OtherSocket)[0];
                return;
            }
            
            var isValid = IsPositionValid();
            collidersOverllaping.Clear();

            if (isValid == false)
            {
                Visible = true;
                ColorBlock(false);
                Snap = new (Socket ThisSocket, Socket OtherSocket)[0];
                return;
            }
            
            var blockSockets = blockGroup.GetComponentsInChildren<Socket>().ToSet();

            var cloneSockets = GetComponentsInChildren<Socket>();
            var candidates = cloneSockets
                .Select(s => (ThisSocket: s, OtherSocket: s.Trigger().FirstOrDefault()))
                .Where(pair => pair.OtherSocket != null)
                .Where(pair => blockSockets.Contains(pair.OtherSocket) == false)
                .ToArray();

            foreach (var cloneSocket in cloneSockets)
            {
                cloneSocket.transform.GetChild(0).gameObject.SetActive(false);
            }

            Snap = candidates
                .Where(pair =>
                {
                    var (thisSocket, otherSocket) = pair;
                    var distance = otherSocket.transform.position.Distance(thisSocket.transform.position);
                    return distance < LockDistanceEpsilon;
                })
                .Select(pair =>
                {
                    var (thisSocket, otherSocket) = pair;
                    return (cloneToBlockMapping[thisSocket], otherSocket);
                })
                .ToArray();

            if (Snap.Length < 2)
            {
                ColorBlock(false);
                Visible = false;
                Snap = new (Socket ThisSocket, Socket OtherSocket)[0];
                return;
            }

            foreach (var pair in Snap)
            {
                pair.ThisSocket.transform.GetChild(0).gameObject.SetActive(true);
            }
            
            Visible = true;
            ColorBlock(true);
        }

        private void ColorBlock(bool isValid)
        {
            var color = isValid ? Color.blue : Color.red;
            var component = GetComponent<Outline>();
            component.OutlineColor = color;
            component.UpdateMaterialProperties();
            material.color = color.SetAlpha(.25f);
        }

        private bool IsPositionValid()
        {
            return collidersOverllaping
                .Select(c => c.GetComponentInParent<BlockGroup>())
                .None(b => b != blockGroup);
        }

        private HashSet<Collider> collidersOverllaping = new HashSet<Collider>();

        private void OnTriggerStay(Collider other)
        {
            collidersOverllaping.Add(other);
        }

        public void BeginSnap()
        {
            gameObject.SetActive(true);
            SwitchLayerInChildren(transform, "Default", "Snap");
        }

        public void EndSnap()
        {
            if (Snap.Any())
            {
                // Copy the transform back
                blockGroup.transform.CopyWorldFrom(transform);
                
                var newBlock = BlockFactory.ConnectBlocks(Snap);
                SwitchLayerInChildren(newBlock.transform, "Snap", "Default");
            }
            else
            {
                transform.GetComponent<Rigidbody>().isKinematic = false;
                SwitchLayerInChildren(transform, "Snap", "Default");
                gameObject.SetActive(false);
            }
        }

        private bool? visible;
        private Renderer[] renderers;
        private Material material;

        private bool Visible
        {
            get => visible.Value;
            set
            {
                if (value != visible)
                {
                    foreach (var renderer in renderers)
                    {
                        renderer.enabled = value;
                    }
                    
                    visible = value;
                }
            }
        }

        private void SwitchLayerInChildren(Transform target, string from, string to)
        {
            foreach (var child in target.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.layer == LayerMask.NameToLayer(from))
                {
                    child.gameObject.layer = LayerMask.NameToLayer(to);
                }
            }
        }

        private bool TrySnap()
        {
            var connections = blockGroup.GetConnections();

            if (connections.Length < 2) return false;
            
            // Chose two closes connections and choose origin and alignment.
            var thisSocketA = connections[0].thisSocket;
            var otherSocketA = connections[0].otherSocket;
            var thisSocketB = connections[1].thisSocket;
            var otherSocketB = connections[1].otherSocket;
            
            var directionA = otherSocketA.up;
            var directionB = otherSocketA.position - otherSocketB.position;

            // Check if the directions are collinear
            if (Math.Abs(directionA.Dot(directionB)) > 0.0001f)
            {
                if (connections.Length < 3) return false;
                
                thisSocketB = connections[2].thisSocket;
                otherSocketB = connections[2].otherSocket;
            }
            
            Align(thisSocketA, thisSocketB, otherSocketA, otherSocketB, blockGroup.transform);
            
            return true;
        }

        private void Align(Transform thisA, Transform thisB, Transform otherA, Transform otherB, Transform block)
        {
            var thisDir = (thisB.position - thisA.position).normalized;
            var otherDir = (otherB.position - otherA.position).normalized;

            var thisToOtherRotation = Quaternion.FromToRotation(thisDir, otherDir);

            // Correct for rotation along the direction (multiple valid states for resulting rotation)
            var correctedUpVector = thisToOtherRotation * -thisA.up;
            var angle = Vector3.SignedAngle(correctedUpVector, otherA.up, otherDir);
            var correction = Quaternion.AngleAxis(angle, otherDir);

            var targetRotation = correction * thisToOtherRotation * block.rotation;

            transform.rotation = targetRotation;
            
            // TODO Get the resulting position and rotation without touching the transforms
            // Adjust position
            transform.position = block.position;
            var blockSocketLocalPosition = block.InverseTransformPoint(thisA.position);
            var adjustedWorldPosition = transform.TransformPoint(blockSocketLocalPosition);
            var offset = otherA.position - adjustedWorldPosition;
            transform.position += offset;
        }
        
        public static SnapPreview CreateFromBlock(BlockGroup blockGroup)
        {
            var (instance, mappings) = SaveMappings(blockGroup);

            // Remove useless components
            instance.GetComponentsInChildren<Component>().Foreach(c =>
            {
                if (c is Transform) return;
                if (c is MeshFilter) return;
                if (c is Renderer) return;
                if (c is Rigidbody) return;
                if (c is Socket) return;
                if (c is Collider && c.GetComponent<Block>()) return;

                DestroyImmediate(c);
            });

            instance.GetComponent<Rigidbody>().isKinematic = true;

            foreach (var collider in instance.GetComponentsInChildren<Collider>())
            {
                if (collider is BoxCollider == false)
                {
                    throw new InvalidOperationException("Only box colliders are supported at this time.");
                }

                collider.isTrigger = true;
                (collider as BoxCollider).size -= Vector3.one * .1f;
            }
            
            var material = new Material(Shader.Find("Standard"));
            material.SetupMaterialWithBlendMode(MaterialExtensions.Mode.Fade);
            foreach (var meshRenderer in instance.GetComponentsInChildren<Renderer>())
            {
                meshRenderer.material = material;
            }
                
            var addComponent = instance.gameObject.AddComponent<Outline>();
            addComponent.OutlineMode = Outline.Mode.OutlineAll;

            var blockClone = instance.AddComponent<SnapPreview>();
            
            blockClone.blockGroup = blockGroup;
            blockClone.material = material;
            blockClone.cloneToBlockMapping = mappings;
            blockClone.renderers = blockClone.GetComponentsInChildren<Renderer>();
            blockClone.Visible = false;
            blockClone.gameObject.SetActive(false);

            return blockClone;
        }

        private class SocketTag : MonoBehaviour
        {
            public string id;
        }
        
        // Make sure to tag the sockets so we know which socket on the preview correspond to the block one.
        private static (GameObject clone, Dictionary<Socket, Socket> mappings) SaveMappings(BlockGroup blockGroup)
        {
            var dict = new Dictionary<string, SocketTag>();

            foreach (var socket in blockGroup.GetComponentsInChildren<Socket>())
            {
                var id = Guid.NewGuid().ToString();
                var socketTag = socket.gameObject.AddComponent<SocketTag>();
                socketTag.id = id;
                dict[id] = socketTag;
            }

            var instance = Instantiate(blockGroup.gameObject);

            var mappings = instance
                .GetComponentsInChildren<SocketTag>()
                .Select(tag =>
                {
                    var id = tag.id;

                    var clone = tag.GetComponent<Socket>();
                    var block2 = dict[id].GetComponent<Socket>();

                    Destroy(tag);
                    Destroy(dict[id]);

                    clone.Owner = block2.Owner;

                    return (clone, block2);
                })
                .ToDictionary(tuple => tuple.clone, tuple => tuple.block2);
            return (instance, mappings);
        }
    }
}