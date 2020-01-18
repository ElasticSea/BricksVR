using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts
{
    public class BlockLink : MonoBehaviour
    {
        private HashSet<BlockLink> connections = new HashSet<BlockLink>();

        public void Connect(BlockLink link)
        {
            link.Add(this);
            this.Add(link);
        }

        private void Add(BlockLink link)
        {
            connections.Add(link);
        }

        public HashSet<BlockLink> GetAllLinks()
        {
            var allConnections = new HashSet<BlockLink>();
            allConnections.Add(this);
            GetAllLinks(this, allConnections);
            return allConnections;
        }

        private void GetAllLinks(BlockLink parent, HashSet<BlockLink> allConnections)
        {
            foreach (var connection in parent.connections)
            {
                if (!allConnections.Contains(connection))
                {
                    allConnections.Add(connection);
                    GetAllLinks(connection, allConnections);
                }
            }
        }

        public HashSet<(BlockLink,BlockLink)> GetAllEdges()
        {
            var allConnections = new HashSet<(BlockLink,BlockLink)>();
            GetAllEdges(this, allConnections);
            return allConnections;
        }

        private void GetAllEdges(BlockLink parent, HashSet<(BlockLink,BlockLink)> allConnections)
        {
            foreach (var connection in parent.connections)
            {
                var edge = (parent, connection);
                if (!allConnections.Contains(edge))
                {
                    allConnections.Add(edge);
                    GetAllEdges(connection, allConnections);
                }
            }
        }
    }
}