using UnityEngine;

namespace _Project.Scripts
{
    public class BlockGrab : MonoBehaviour
    {
        [SerializeField] private Grabbable grabbable;
        [SerializeField] private Block block;

        private void Awake()
        {
            grabbable.OnGrabBegin += delegate((OVRGrabber hand, Collider grabPoint) tuple)
            {
                block.BeginSnap();
            };
            
            grabbable.OnGrabEnd += delegate((Vector3 linearVelocity, Vector3 angularVelocity) tuple)
            {
                block.EndSnap();
            };
        }

        public Block Block
        {
            set => block = value;
        }

        public void SetupGrabPoints(Collider[] colliders)
        {
            grabbable.SetupGrabPoints(colliders);
        }
    }
}