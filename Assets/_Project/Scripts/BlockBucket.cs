using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts
{
    public class BlockBucket : MonoBehaviour
    {
        [SerializeField] private Block blockPrefab;
        [SerializeField] private BoxCollider insideTrigger;

        private Block lastInstance;
        private void Update()
        {
            var blocksInside = insideTrigger.OverlapBox();

            var isInside = blocksInside.Any(b => b.transform == lastInstance?.transform);
            if (isInside == false)
            {
                lastInstance = Instantiate(blockPrefab, null, false);
                var placementCenter = insideTrigger.transform.TransformPoint(insideTrigger.center);
                var brickSize = lastInstance.transform.TransformVector(lastInstance.GetComponent<BoxCollider>().size);
                lastInstance.transform.position = placementCenter - brickSize / 2;
            }
        }
    }
}