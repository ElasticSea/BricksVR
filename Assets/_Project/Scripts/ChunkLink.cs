using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts
{
    public class ChunkLink : MonoBehaviour
    {
        [SerializeField] private bool isLinkAnchored;
        
        private HashSet<ChunkLink> connections = new HashSet<ChunkLink>();

        public List<ChunkLink> Connections => connections.ToList();

        public void Connect(ChunkLink link)
        {
            link.Add(this);
            this.Add(link);
        }
        
        public void Disconnect(ChunkLink link)
        {
            link.Remove(this);
            this.Remove(link);
        }

        private void Add(ChunkLink link)
        {
            connections.Add(link);
        }
        
        private void Remove(ChunkLink link)
        {
            connections.Remove(link);
        }

        public HashSet<ChunkLink> GetAllLinks()
        {
            var allConnections = new HashSet<ChunkLink>();
            allConnections.Add(this);
            GetAllLinks(this, allConnections, null);
            return allConnections;
        }
        
        private HashSet<ChunkLink> GetAllLinks(ISet<ChunkLink> ignore)
        {
            var allConnections = new HashSet<ChunkLink>();
            allConnections.Add(this);
            GetAllLinks(this, allConnections, ignore);
            return allConnections;
        }

        private void GetAllLinks(ChunkLink parent, HashSet<ChunkLink> allConnections, ISet<ChunkLink> ignore)
        {
            foreach (var connection in parent.connections)
            {
                if (ignore != null && ignore.Contains(connection))
                {
                    continue;                    
                }
                
                if (!allConnections.Contains(connection))
                {
                    allConnections.Add(connection);
                    GetAllLinks(connection, allConnections, ignore);
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

        public bool IsChunkAnchored => GetAllLinks().Any(l => l.isLinkAnchored);

        public bool IsLinkAnchored
        {
            get => isLinkAnchored;
            set => isLinkAnchored = value;
        }

        public List<ISet<ChunkLink>> SplitBy(ISet<ChunkLink> chunk)
        {
            var all = GetAllLinks().ToList();
            var rest = all.Except(chunk).ToList();
            foreach (var link in rest)
            {
                foreach (var chunkLink in chunk)
                {
                    chunkLink.Disconnect(link);
                }
            }
            var groups = new List<ISet<ChunkLink>>();
            
            groups.Add(chunk);
            
            while (rest.Any())
            {
                var first = rest.First();
                var groupA = first.GetAllLinks(chunk);
                rest = rest.Except(groupA).ToList();
                groups.Add(groupA);
            }

            return groups;
        }
    }
}