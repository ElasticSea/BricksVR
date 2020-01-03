using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Framework.Scripts.Extensions;

namespace _Framework.Scripts.Util.Triggers
{
    public abstract class PreciseTrigger<T> : MonoBehaviour where T : Component
    {
        private readonly ISet<T> current = new HashSet<T>();
        private ISet<T> last = new HashSet<T>();

        private void OnTriggerStay(Collider other)
        {
            var component = other.GetComponent<T>();
            if (component && other.isTrigger == false)
            {
                if (current.Contains(component) == false)
                {
                    current.Add(component);
                }
            }
        }

        private void FixedUpdate()
        {
            foreach (var occupant in current.Where(c => last.Contains(c) == false))
            {
                OnEnter(occupant);
            }

            foreach (var occupant in last.Where(c => current.Contains(c) == false))
            {
                OnExit(occupant);
            }

            if (last.Count == 0 && current.Count > 0)
            {
                OnFirstEnter();
            }

            if (last.Count > 0 && current.Count == 0)
            {
                OnLastExit();
            }

            last = current.ToSet();
            current.Clear();
        }

        protected virtual void OnEnter(T occupant)
        {
        }

        protected virtual void OnExit(T occupant)
        {
        }

        protected virtual void OnFirstEnter()
        {
        }

        protected virtual void OnLastExit()
        {
        }
    }
}