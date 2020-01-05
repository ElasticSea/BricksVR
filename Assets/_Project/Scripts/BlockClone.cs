using System;
using System.Collections.Generic;
using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts
{
    public class BlockClone : MonoBehaviour
    {
        public event Action<LockResult> OnLocked = result => { };

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
                sphere.transform.localScale = Vector3.one * socket.Radius;
                var mat = sphere.GetComponent<Renderer>().material;
                mat.color = Color.blue;
                
                sphere.transform.SetParent(cloneSocket.transform, false);
            }
        }
        
        private Block block;

        private const float LOCK_DISTANCE = 0.01f;

        private void Update()
        {
            var blockSockets = block.GetComponentsInChildren<Socket>().ToSet();

            var cloneSockets = GetComponentsInChildren<Socket>();
            var candidates = cloneSockets
                .Select(s => new {Socket=s, Connection=s.Trigger().FirstOrDefault()})
                .Where(pair => pair.Connection != null)
                .Where(pair => blockSockets.Contains(pair.Connection) == false)
                .ToArray();

            foreach (var cloneSocket in cloneSockets)
            {
                cloneSocket.transform.GetChild(0).gameObject.SetActive(false);
            }

            var locked = candidates.Where(pair =>
            {
                var distance = pair.Connection.transform.position.Distance(pair.Socket.transform.position);
                return distance < LOCK_DISTANCE;
            }).ToArray();


            foreach (var pair in locked)
            {
                pair.Socket.transform.GetChild(0).gameObject.SetActive(true);
            }

            if (OVRInput.Get(OVRInput.RawButton.X))
            {
                var result = new LockResult
                {
                    BlockA = block,
                    BlockASockets = locked.Select(pair => cloneToBlockMapping[pair.Socket]).ToArray(),
                    BlockB = locked.FirstOrDefault()?.Connection.GetComponentInParent<Block>(),
                    BlockBSockets = locked.Select(pair => pair.Connection).ToArray()
                };

                OnLocked(result);
            }
        }
    }

    public class LockResult
    {
        public Block BlockA;
        public Socket[] BlockASockets;
        public Block BlockB;
        public Socket[] BlockBSockets;
    }
}