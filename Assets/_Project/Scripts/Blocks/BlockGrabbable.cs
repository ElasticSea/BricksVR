using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Blocks
{
    public class BlockGrabbable : MonoBehaviour
    {
        [SerializeField] private BlockGroup blockGroup;

        private void Awake()
        {
            var grabbable = GetComponent<Grabbable>();
            grabbable.OnGrabBegin += obj => blockGroup.BeginSnap();
            grabbable.OnGrabEnd += obj => blockGroup.EndSnap();
        }

        public BlockGroup BlockGroup
        {
            set => blockGroup = value;
        }

        public void SetupGrabPoints(Collider[] colliders)
        {
            var grabbable = GetComponent<Grabbable>();
            grabbable.SetupGrabPoints(colliders.Where(c => c).ToArray());
        }
    }
}