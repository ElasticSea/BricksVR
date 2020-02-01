using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts
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

                var links = hits.Select(h => h.GetComponent<ChunkLink>()).Where(it => it);
                BlockFactory.DisconnectChunk(links);
            }
        }
    }
}