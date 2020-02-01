using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts.Blocks
{
    public class BlockSplitter : MonoBehaviour
    {
        [SerializeField] private OVRInput.Controller controller;
        [SerializeField] private SphereCollider splitVolume;
        
        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.One, controller))
            {
                var hits = splitVolume.OverlapSphere();

                var links = hits.Select(h => h.GetComponent<Block>()).Where(it => it);
                
                // Only disconnects first group
                var links2 = links.GroupBy(l => l.GetComponentInParent<BlockGroup>()).First();
                
                BlockFactory.DisconnectChunk(links2.Key, links2.ToList());
            }
        }
    }
}