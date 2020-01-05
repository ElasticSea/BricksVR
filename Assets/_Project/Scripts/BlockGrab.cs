using UnityEngine;

namespace _Project.Scripts
{
    public class BlockGrab : OVRGrabbable
    {
        [SerializeField] private Block block;
        
        public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            base.GrabBegin(hand, grabPoint);
            block.CloneActive = true;
        }

        public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            base.GrabEnd(linearVelocity, angularVelocity);
            block.CloneActive = false;
        }
    }
}