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
        private Dictionary<Socket, Socket> cloneToBlockMapping = new Dictionary<Socket, Socket>();
        public void Setup(Block block)
        {
            this.block = block;
            gameObject.AddComponent<MeshFilter>().mesh = block.GetComponent<MeshFilter>().mesh;

            var material = new Material(Shader.Find("Standard"));
            material.color = Color.blue.SetAlpha(0.15f);
            material.SetupMaterialWithBlendMode(MaterialExtensions.Mode.Fade);
            gameObject.AddComponent<MeshRenderer>().material = material;
            var addComponent = gameObject.AddComponent<Outline>();
            addComponent.OutlineMode = Outline.Mode.OutlineAll;
            addComponent.OutlineColor = Color.blue;

            gameObject.transform.CopyWorldFrom(block.transform);
            gameObject.SetActive(false);

            foreach (var socket in block.GetComponentsInChildren<Socket>())
            {
                var cloneSocket = new GameObject().AddComponent<Socket>();
                cloneSocket.Radius = socket.Radius;
                cloneSocket.Type = socket.Type;
                cloneSocket.Active = false;
                cloneSocket.transform.SetParent(gameObject.transform, false);
                cloneSocket.transform.position = socket.transform.position;
                cloneSocket.transform.rotation = socket.transform.rotation;
                cloneToBlockMapping[cloneSocket] = socket;

                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(sphere.GetComponent<Collider>());
                sphere.transform.localScale = Vector3.one * socket.Radius * 2;
                var mat = sphere.GetComponent<Renderer>().material;
                mat.color = Color.blue;
                
                sphere.transform.SetParent(cloneSocket.transform, false);
            }
        }
        
        private Block block;
        private SocketPair[] locked;

        private const float LockDistanceEpsilon = 1E-06f;

        private void Update()
        {
            var blockSockets = block.GetComponentsInChildren<Socket>().ToSet();

            var cloneSockets = GetComponentsInChildren<Socket>();
            var candidates = cloneSockets
                .Select(s => new SocketPair {Socket=s, Connection=s.Trigger().FirstOrDefault()})
                .Where(pair => pair.Connection != null)
                .Where(pair => blockSockets.Contains(pair.Connection) == false)
                .ToArray();

            foreach (var cloneSocket in cloneSockets)
            {
                cloneSocket.transform.GetChild(0).gameObject.SetActive(false);
            }

            locked = candidates.Where(pair =>
            {
                var distance = pair.Connection.transform.position.Distance(pair.Socket.transform.position);
                print(distance);
                return distance < LockDistanceEpsilon;
            }).ToArray();


            foreach (var pair in locked)
            {
                pair.Socket.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            locked = new SocketPair[0];
        }

        public LockResult GetLock()
        {
            return new LockResult
            {
                BlockA = block.GetComponent<BlockLink>(),
                BlockASockets = locked.Select(pair => cloneToBlockMapping[pair.Socket]).ToArray(),
                BlockB = locked.FirstOrDefault()?.Connection.GetComponentInParent<BlockLink>(),
                BlockBSockets = locked.Select(pair => pair.Connection).ToArray()
            };
        }
    }

    public class LockResult
    {
        public BlockLink BlockA;
        public Socket[] BlockASockets;
        public BlockLink BlockB;
        public Socket[] BlockBSockets;
    }

    public class SocketPair
    {
        public Socket Socket;
        public Socket Connection;
    }
}