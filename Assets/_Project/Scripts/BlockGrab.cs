using UnityEngine;

namespace _Project.Scripts
{
    public class BlockGrab : OVRGrabbable
    {
        [SerializeField] private Block block;

        public Block Block
        {
            get => block;
            set => block = value;
        }

        public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            base.GrabBegin(hand, grabPoint);
            block.BeginSnap();
        }

        public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            base.GrabEnd(linearVelocity, angularVelocity);
            block.EndSnap();
        }

        public void SetupGrabPoints(Collider[] colliders)
        {
            m_grabPoints = colliders;
        }
    }
}