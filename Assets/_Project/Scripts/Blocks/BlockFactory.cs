using System.Collections.Generic;
using System.Linq;
using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts.Blocks
{
    public static class BlockFactory
    {
        public static BlockGroup ConnectBlocks((Socket ThisSocket, Socket OtherSocket)[] result)
        {
            // Connect The sockets
            foreach (var (thisSocket, otherSocket) in result)
            {
                thisSocket.Connect(otherSocket);
            }

            return ConnectGroups(result.First().ThisSocket.Owner.GetBlocksInGroup());
        }

        private static BlockGroup ConnectGroups(IEnumerable<Block> blocks)
        {
            // Get all connected blocks
            var allGroups = blocks
                .Select(socket => socket.GetComponentInParent<BlockGroup>().gameObject)
                .Distinct()
                .ToList();

            var allBlocks = allGroups.SelectMany(b => b.transform.Children()).ToList();
        
            var newGroup = CreateGroup(allBlocks, allGroups[0].transform);

            foreach (var block in allGroups)
            {
                Object.DestroyImmediate(block.gameObject);
            }
            
            return newGroup;
        }

        private static BlockGroup CreateGroup(IEnumerable<Component> blocks, Transform blockGroup)
        {
            // Reparent blocks
            var newParent = new GameObject();
            newParent.transform.SetParent(blockGroup.parent);
            newParent.transform.CopyLocalFrom(blockGroup);

            foreach (var link in blocks)
            {
                link.transform.SetParent(newParent.transform, true);

                // Snap position & rotation
                link.transform.localPosition = link.transform.localPosition.Snap(0.0001f);
                link.transform.localRotation = Quaternion.Euler(link.transform.localRotation.eulerAngles.Snap(45f));
            }

            // Create a new block
            var blk = newParent.AddComponent<BlockGroup>();

            // Check if block is anchored and set the rigidbody appropriately
            var isAnchored = blk.GetComponentInChildren<Block>().IsChunkAnchored;
            if (isAnchored == false)
            {
                newParent.AddComponent<Grabbable>();
                var blockGrab = newParent.AddComponent<BlockGrabbable>();
                blockGrab.BlockGroup = blk;
                blockGrab.SetupGrabPoints(newParent.GetComponentsInChildren<Collider>());
            }

            var newRb = newParent.AddComponent<Rigidbody>();
            newRb.interpolation = RigidbodyInterpolation.Interpolate;
            newRb.isKinematic = isAnchored;
            return blk;
        }

        public static List<BlockGroup> DisconnectChunk(BlockGroup group, IEnumerable<Block> chunk)
        {
            if (chunk.None())
                return new List<BlockGroup>();

            var newBlocks = group
                .SplitBy(chunk.ToSet())
                .Select((part, i) => 
                {
                    var currentGroup = part.First().GetComponentInParent<BlockGroup>().gameObject;
                    var newGroup = CreateGroup(part, currentGroup.transform);;
                    newGroup.name = $"{group.name} [{i}]";
                    return newGroup;
                })
                .ToList();
            
            Object.Destroy(group.gameObject);

            return newBlocks;
        }
    }
}