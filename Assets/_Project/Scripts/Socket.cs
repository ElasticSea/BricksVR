using System;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts
{
    public class Socket : MonoBehaviour
    {
        [SerializeField] private SocketType type;
        [SerializeField] private float radius = 0.125f;
        [SerializeField] private bool active = true;
        
        private SphereCollider trigger;

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

        public bool Active
        {
            get => active;
            set
            {
                active = value;
                if(trigger) trigger.enabled = value;
            }
        }

        private void Start()
        {
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = trigger;
            Radius = radius;
            Active = active;
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
                .ToArray();
        }

        private void OnDrawGizmos()
        {
            if (Active == false) return;
            
            var candidates = Trigger();
            foreach (var candidate in candidates)
            {
                Gizmos.color = Color.yellow.SetAlpha(.25f);
                Gizmos.DrawLine(transform.position, candidate.transform.position);
            }

            var color = Type == SocketType.Male ? Color.red : Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = color.SetAlpha(.25f);
            Gizmos.DrawSphere(Vector3.zero, Radius);
            Gizmos.color = Color.white.SetAlpha(.25f);
            Gizmos.DrawLine(Vector3.zero, Vector3.up * .1f);
        }
    }
}