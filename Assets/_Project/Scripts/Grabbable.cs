using System;
using UnityEngine;

namespace _Project.Scripts
{
    public class Grabbable : OVRGrabbable
    {
        public event Action<(OVRGrabber hand, Collider grabPoint)> OnGrabBegin = tuple => { };
        public event Action<(Vector3 linearVelocity, Vector3 angularVelocity)> OnGrabEnd = tuple => { };
        
        public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            m_grabbedKinematic = false;
            base.GrabBegin(hand, grabPoint);
            OnGrabBegin((hand, grabPoint));
        }

        public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            base.GrabEnd(linearVelocity, angularVelocity);
            OnGrabEnd((linearVelocity, angularVelocity));
        }

        public void SetupGrabPoints(Collider[] colliders)
        {
            m_grabPoints = colliders;
        }
    }
}