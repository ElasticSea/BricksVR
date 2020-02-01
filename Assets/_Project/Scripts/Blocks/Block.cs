using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Blocks
{
    [ExecuteInEditMode]
    public class Block : MonoBehaviour
    {
        [SerializeField] private bool isLinkAnchored;
        [SerializeField] private Socket[] sockets;

        private HashSet<Block> connections
        {
            get
            {
                var set = new HashSet<Block>();
                for (var i = 0; i < sockets.Length; i++)
                {
                    var socket = sockets[i];
                    if (socket.ConnectedSocket != null)
                    {
                        set.Add(socket.ConnectedSocket.Owner);
                    }
                }

                return set;
            }
        }

        public List<Block> Connections => connections.ToList();
        public List<Socket> Sockets => sockets.ToList();

        private void Awake()
        {
            sockets = GetComponentsInChildren<Socket>();
        }

        public HashSet<Block> GetBlocksInGroup()
        {
            var allConnections = new HashSet<Block>();
            allConnections.Add(this);
            GetBlocksInGroup(this, allConnections, null);
            return allConnections;
        }
        
        public HashSet<Block> GetBlocksInGroup(ISet<Block> ignore)
        {
            var allConnections = new HashSet<Block>();
            allConnections.Add(this);
            GetBlocksInGroup(this, allConnections, ignore);
            return allConnections;
        }

        private void GetBlocksInGroup(Block parent, HashSet<Block> allConnections, ISet<Block> ignore)
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
                    GetBlocksInGroup(connection, allConnections, ignore);
                }
            }
        }

        public HashSet<(Block,Block)> GetAllEdges()
        {
            var allConnections = new HashSet<(Block,Block)>();
            GetAllEdges(this, allConnections);
            return allConnections;
        }

        private void GetAllEdges(Block parent, HashSet<(Block,Block)> allConnections)
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

        public bool IsChunkAnchored => GetBlocksInGroup().Any(l => l.isLinkAnchored);

        public bool IsLinkAnchored
        {
            get => isLinkAnchored;
            set => isLinkAnchored = value;
        }
    }
}