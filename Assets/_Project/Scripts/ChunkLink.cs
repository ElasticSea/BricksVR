using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts
{
    public class ChunkLink : MonoBehaviour
    {
        [SerializeField] private bool anchor;
        
        private HashSet<ChunkLink> connections = new HashSet<ChunkLink>();

        public void Connect(ChunkLink link)
        {
            link.Add(this);
            this.Add(link);
        }

        private void Add(ChunkLink link)
        {
            connections.Add(link);
        }

        public HashSet<ChunkLink> GetAllLinks()
        {
            var allConnections = new HashSet<ChunkLink>();
            allConnections.Add(this);
            GetAllLinks(this, allConnections);
            return allConnections;
        }

        private void GetAllLinks(ChunkLink parent, HashSet<ChunkLink> allConnections)
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

        public HashSet<(ChunkLink,ChunkLink)> GetAllEdges()
        {
            var allConnections = new HashSet<(ChunkLink,ChunkLink)>();
            GetAllEdges(this, allConnections);
            return allConnections;
        }

        private void GetAllEdges(ChunkLink parent, HashSet<(ChunkLink,ChunkLink)> allConnections)
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

        public bool IsAnchored => GetAllLinks().Any(l => l.anchor);

        public bool Anchor
        {
            get => anchor;
            set => anchor = value;
        }
    }
}