using System.Collections.Generic;
using UnityEngine;
using _Framework.Scripts.Extensions;

namespace _Framework.Scripts.Util
{
    public class Trigger<T> : MonoBehaviour where T : Component
    {
        protected readonly HashSet<Collider> occupants = new HashSet<Collider>();

        public ISet<Collider> Occupants => occupants.ToSet();

        public void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<T>() && other.isTrigger == false)
            {
                occupants.Add(other);

                if (occupants.Count == 1)
                {
                    OnFirstEnter(other);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (occupants.Contains(other))
            {
                occupants.Remove(other);

                if (occupants.Count == 0)
                {
                    OnLastExit(other);
                }
            }
        }

        protected virtual void OnFirstEnter(Collider other)
        {
        }

        protected virtual void OnLastExit(Collider other)
        {
        }
    }
}