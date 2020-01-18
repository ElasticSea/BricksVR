using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts
{
    public class BlockClone : MonoBehaviour
    {
        private Block block;
        private Dictionary<Socket, Socket> cloneToBlockMapping;
        public (Socket ThisSocket, Socket OtherSocket)[] Locked { get; private set; }

        public Block Block
        {
            get => block;
            set => block = value;
        }

        public Dictionary<Socket, Socket> CloneToBlockMapping
        {
            get => cloneToBlockMapping;
            set => cloneToBlockMapping = value;
        }

        private const float LockDistanceEpsilon = 1E-06f;

        private void Update()
        {
            var isValid = IsPositionValid();
            collidersOverllaping.Clear();

            var color = isValid ? Color.blue : Color.red;
            foreach (var outline in GetComponentsInChildren<Outline>())
            {
                outline.OutlineColor = color;
            }

            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                foreach (var mat in renderer.materials)
                {
                    mat.color = color.SetAlpha(.4f);
                }
            }

            if (isValid == false)
            {
                Locked = new (Socket ThisSocket, Socket OtherSocket)[0];
                return;
            }
            
            var blockSockets = block.GetComponentsInChildren<Socket>().ToSet();

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

            Locked = candidates
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


            foreach (var pair in Locked)
            {
                pair.ThisSocket.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        private class SocketTag : MonoBehaviour
        {
            public string id;
        }

        public static BlockClone SetupBlock(Block block)
        {
            var dict = new Dictionary<string, SocketTag>();
              
            foreach (var socket in block.GetComponentsInChildren<Socket>())
            {
                var id = Guid.NewGuid().ToString();
                var socketTag = socket.gameObject.AddComponent<SocketTag>();
                socketTag.id = id;
                dict[id] = socketTag;
            }

            var blockClone = Instantiate(block.gameObject).AddComponent<BlockClone>();
            Destroy(blockClone.GetComponent<Block>());

            foreach (var collider in blockClone.GetComponentsInChildren<Collider>())
            {
                if (collider.GetComponent<BlockLink>() == false)
                {
                    DestroyImmediate(collider);                    
                }
            }
            
            foreach (var link in blockClone.GetComponentsInChildren<BlockLink>())
            {
                var collider = link.GetComponent<Collider>();
                if (collider is BoxCollider == false)
                {
                    throw new InvalidOperationException("Only box colliders are supported at this time.");
                }

                collider.isTrigger = true;
                (collider as BoxCollider).size -= Vector3.one * .1f;
            }
            
            foreach (var gr in blockClone.GetComponentsInChildren<BlockGrab>())
            {
                Destroy(gr);
            }

            blockClone.GetComponent<Rigidbody>().isKinematic = true;

            blockClone.Block = block;
            blockClone.CloneToBlockMapping = blockClone
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

            foreach (var meshRenderer in blockClone.GetComponentsInChildren<MeshRenderer>())
            {
                var material = new Material(Shader.Find("Standard"));
                material.SetupMaterialWithBlendMode(MaterialExtensions.Mode.Fade);
                meshRenderer.material = material;
                
                var addComponent = meshRenderer.gameObject.AddComponent<Outline>();
                addComponent.OutlineMode = Outline.Mode.OutlineAll;
            }

            blockClone.gameObject.SetActive(false);

            foreach (var socket in blockClone.GetComponentsInChildren<Socket>())
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(sphere.GetComponent<Collider>());
                sphere.transform.localScale = Vector3.one * socket.Radius * 2;
                var mat = sphere.GetComponent<Renderer>().material;
                mat.color = Color.blue;

                sphere.transform.SetParent(socket.transform, false);
            }

            return blockClone;
        }

        private bool IsPositionValid()
        {
            return collidersOverllaping
                .Select(c => c.GetComponentInParent<Block>())
                .None(b => b != block);
        }

        private HashSet<Collider> collidersOverllaping = new HashSet<Collider>();

        private void OnTriggerStay(Collider other)
        {
            collidersOverllaping.Add(other);
        }

        private void OnDisable()
        {
            Locked = new (Socket ThisSocket, Socket OtherSocket)[0];
        }
    }
}