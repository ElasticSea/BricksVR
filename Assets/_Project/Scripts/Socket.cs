using System;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts
{
    public class Socket : MonoBehaviour
    {
        [SerializeField] private Block owner;
        [SerializeField] private SocketType type;
        [SerializeField] private float radius = 0.125f;
        
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

        private void Awake()
        {
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = trigger;
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