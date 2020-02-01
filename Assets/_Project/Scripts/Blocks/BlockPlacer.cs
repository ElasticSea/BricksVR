using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts.Blocks
{
    public class BlockPlacer : MonoBehaviour
    {
        [SerializeField] private BlockGroup[] blocks;
        [SerializeField] private float offset = .25f;

        private void Awake()
        {
            var position = Vector3.zero;
            
            foreach (var block in blocks)
            {
                position = Spawn(block, position);
            }
        }

        private Vector3 Spawn(BlockGroup blockGroupPrefab, Vector3 position)
        {
            var instance = Instantiate(blockGroupPrefab, transform, false);
            var rb = instance.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            instance.transform.localPosition = position;

            var bounds = instance.gameObject.GetCompositeRendererBounds();
            var positionCopy = position;
            position += Vector3.forward.Multiply(bounds.size + Vector3.one * offset);
            
            instance.GetComponent<Grabbable>().OnGrabBegin += tuple =>
            {
                Spawn(blockGroupPrefab, positionCopy);
            };
            return position;
        }
    }
}