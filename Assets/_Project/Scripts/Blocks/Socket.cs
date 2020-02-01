using System;
using System.Linq;
using _Project.Scripts.Blocks;
using UnityEngine;

namespace _Project.Scripts
{
    public class Socket : MonoBehaviour
    {
        [SerializeField] private Block owner;
        [SerializeField] private SocketType type;
        [SerializeField] private float radius = 0.125f;

        [SerializeField] private Socket connectedSocket;
        
        private SphereCollider trigger;

        public Block Owner
        {
            get
            {
                if (owner == null)
                {
                    owner = GetComponentInParent<Block>();

                    if (owner == null)
                    {
                        throw new InvalidOperationException("This socket does not belong to any block. And no parent block could not be found.");
                    }
                }
                return owner;
            }
            set => owner = value;
        }

        public SocketType Type
        {
            get => type;
            set => type = value;
        }

        public float Radius
        {
            get => radius;
            set
            {
                radius = value;
                if(trigger) trigger.radius = value;
            }
        }

        public Socket ConnectedSocket => connectedSocket;

        private void Awake()
        {
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = trigger;
            trigger.enabled = connectedSocket == false;
            Radius = radius;
        }

        public Socket[] Trigger()
        {
            var position = transform.position;
            var radius = Radius * transform.lossyScale.x;
            var layerMask = LayerMask.GetMask("Socket");
            var candidates = Physics.OverlapSphere(position, radius, layerMask);

            return candidates
                .Select(c => c.GetComponent<Socket>())
                .Where(s => s.Type != Type)
                .Where(s => s.Owner != Owner)
                .ToArray();
        }
        
        public void Connect(Socket socket)
        {
            if (connectedSocket == null)
            {
                AttachSocket(socket);
                socket.AttachSocket(this);
            }
        }

        private void AttachSocket(Socket other)
        {
            connectedSocket = other;
            trigger.enabled = false;
        }
        
        public void Disconnect()
        {
            if (connectedSocket != null)
            {
                connectedSocket.DetachSocket();
                DetachSocket();
            }
        }
        private void DetachSocket()
        {
            connectedSocket = null;
            trigger.enabled = true;
        }

        private void OnDrawGizmos()
        {
            var candidates = Trigger();
            foreach (var candidate in candidates)
            {
                Gizmos.color = Color.yellow.SetAlpha(.25f);
                Gizmos.DrawLine(transform.position, candidate.transform.position);
            }

            var color = Type == SocketType.Male ? Color.blue : Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = color.SetAlpha(.25f);
            Gizmos.DrawSphere(Vector3.zero, Radius);
            Gizmos.color = Color.white.SetAlpha(.25f);
            Gizmos.DrawLine(Vector3.zero, Vector3.up * .1f);
        }
    }
}