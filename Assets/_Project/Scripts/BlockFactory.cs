using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts
{
    public static class BlockFactory
    {
        public static Block ConnectBlocks((Socket ThisSocket, Socket OtherSocket)[] result)
        {
            ChunkLink chunkLink = null;
            
            // Connect The Links and destroy the sockets
            foreach (var (thisSocket, otherSocket) in result)
            {
                var thisLink = thisSocket.GetComponentInParent<ChunkLink>();
                var otherLink = otherSocket.GetComponentInParent<ChunkLink>();
                Object.DestroyImmediate(thisSocket.gameObject);
                Object.DestroyImmediate(otherSocket.gameObject);
                thisLink.Connect(otherLink);

                chunkLink = thisLink;
            }

            // Get all connected blocks
            var allBlocks = chunkLink.GetAllLinks()
                .Select(socket => socket.GetComponentInParent<Block>().gameObject)
                .Distinct()
                .ToList();

            // Reparent blocks
            var newParent = new GameObject();
            newParent.transform.SetParent(allBlocks[0].transform.parent);
            newParent.transform.CopyLocalFrom(allBlocks[0].transform);
            
            foreach (var block in allBlocks)
            {
                foreach (var child in block.transform.Children())
                {
                    child.transform.SetParent(newParent.transform, true);
                    
                    // Snap position & rotation
                    child.transform.localPosition = child.transform.localPosition.Snap(0.0001f);
                    child.transform.localRotation = Quaternion.Euler(child.transform.localRotation.eulerAngles.Snap(45f));
                }
                Object.DestroyImmediate(block.gameObject);
            }
       
            // Create a new block
            var blk = newParent.AddComponent<Block>();

            // Check if block is anchored and set the rigidbody appropriately
            var isAnchored = blk.GetComponentInChildren<ChunkLink>().IsAnchored;
            if (isAnchored == false)
            {
                var blockGrab = newParent.AddComponent<BlockGrab>();
                blockGrab.Block = blk;
                blockGrab.SetupGrabPoints(newParent.GetComponentsInChildren<Collider>());
            }

            var newRb = newParent.AddComponent<Rigidbody>();
            newRb.interpolation = RigidbodyInterpolation.Interpolate;
            newRb.isKinematic = isAnchored;
            
            return blk;
        }
    }
}