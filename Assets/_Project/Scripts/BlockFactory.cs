using System.Collections.Generic;
using System.Linq;
using _Framework.Scripts.Extensions;
using _Framework.Scripts.Util;
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
//                Object.DestroyImmediate(thisSocket.gameObject);
//                Object.DestroyImmediate(otherSocket.gameObject);
                thisLink.Connect(otherLink);

                chunkLink = thisLink;
            }

            return CreateNewBlock(chunkLink.GetAllLinks());
        }

        private static Block CreateNewBlock(IEnumerable<ChunkLink> chunk)
        {
            // Get all connected blocks
            var allBlocks = chunk
                .Select(socket => socket.GetComponentInParent<Block>().gameObject)
                .Distinct()
                .ToList();

            // Reparent blocks
            var newParent = new GameObject("Block "+chunk.Count());
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
            var isAnchored = blk.GetComponentInChildren<ChunkLink>().IsChunkAnchored;
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
        
        private static Block CreateNewBlock2(IEnumerable<ChunkLink> chunk)
        {
            // Get all connected blocks
            var block = chunk.First().GetComponentInParent<Block>().gameObject;

            // Reparent blocks
            var newParent = new GameObject();
            newParent.transform.SetParent(block.transform.parent);
            newParent.transform.CopyLocalFrom(block.transform);

            foreach (var link in chunk)
            {
                link.transform.SetParent(newParent.transform, true);

                // Snap position & rotation
                link.transform.localPosition = link.transform.localPosition.Snap(0.0001f);
                link.transform.localRotation = Quaternion.Euler(link.transform.localRotation.eulerAngles.Snap(45f));
            }

            // Create a new block
            var blk = newParent.AddComponent<Block>();

            // Check if block is anchored and set the rigidbody appropriately
            var isAnchored = blk.GetComponentInChildren<ChunkLink>().IsChunkAnchored;
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

        public static void DisconnectChunk(IEnumerable<ChunkLink> links)
        {
            if (links.None()) return;
            
            // Only disconnects first group
            var links2 = links.GroupBy(l => l.GetComponentInParent<Block>()).First().ToList();

            var block = links2[0].GetComponentInParent<Block>();
            var parts = links2[0].SplitBy(links.ToSet());

            foreach (var part in parts)
            {
                CreateNewBlock2(part);
            }
            
            Object.Destroy(block.gameObject);
        }
    }
}