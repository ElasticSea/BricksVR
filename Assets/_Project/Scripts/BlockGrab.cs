using System.Linq;
using UnityEngine;

namespace _Project.Scripts
{
    [RequireComponent(typeof(Grabbable))]
    public class BlockGrab : MonoBehaviour
    {
        [SerializeField] private Block block;

        private void Awake()
        {
            var grabbable = GetComponent<Grabbable>();
            grabbable.OnGrabBegin += obj => block.BeginSnap();
            grabbable.OnGrabEnd += obj => block.EndSnap();
        }

        public Block Block
        {
            set => block = value;
        }

        public void SetupGrabPoints(Collider[] colliders)
        {
            var grabbable = GetComponent<Grabbable>();
            grabbable.SetupGrabPoints(colliders.Where(c => c).ToArray());
        }
    }
}