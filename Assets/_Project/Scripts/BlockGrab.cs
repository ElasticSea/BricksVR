using UnityEngine;

namespace _Project.Scripts
{
    public class BlockGrab : OVRGrabbable
    {
        [SerializeField] private Block block;
        
        public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            base.GrabBegin(hand, grabPoint);
            
            foreach (var blockLink in GetComponent<BlockLink>().GetAllLinks())
            {
                blockLink.GetComponent<Block>().BeginSnap();
            }
        }

        public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            base.GrabEnd(linearVelocity, angularVelocity);
            
            foreach (var blockLink in GetComponent<BlockLink>().GetAllLinks())
            {
                blockLink.GetComponent<Block>().EndSnap();
            }
        }
    }
}