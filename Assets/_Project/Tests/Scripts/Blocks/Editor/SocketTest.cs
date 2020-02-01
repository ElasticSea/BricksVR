using System.Collections.Generic;
using System.Linq;
using _Project.Scripts;
using _Project.Scripts.Blocks;
using NUnit.Framework;
using UnityEngine;

namespace _Project.Tests.Scripts.Blocks.Editor
{
    public class SocketTest
    {
        private static (Block block, Socket[] sockets) SetupSockets(params SocketType[] types)
        {
            var go = new GameObject();
            foreach (var type in types)
            {
                go.AddComponent<Socket>().Type = type;
            }

            var block = go.AddComponent<Block>();
            var blockg = go.AddComponent<BlockGroup>();

            return (block, go.GetComponentsInChildren<Socket>());
        }
        
        [Test]
        public void SplitNone()
        {
            var (blockA, socketsA) = SetupSockets();
            
            var group = new HashSet<Block> {};
            var result = blockA.GetComponent<BlockGroup>().SplitBy(group);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(blockA, result[1].First());
            Assert.AreEqual(blockA.Connections.Count, 0);
        }
        
        [Test]
        public void SplitSame()
        {
            var (blockA, socketsA) = SetupSockets();
            
            var group = new HashSet<Block> {blockA};
            var result = blockA.GetComponent<BlockGroup>().SplitBy(group);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(group, result[0]);
            Assert.AreEqual(blockA.Connections.Count, 0);
        }
        
        [Test]
        public void SplitCircleInHalf()
        {
            var (blockA, socketsA) = SetupSockets(SocketType.Male, SocketType.Male);
            var (blockB, socketsB) = SetupSockets(SocketType.Female, SocketType.Female);
            var (blockC, socketsC) = SetupSockets(SocketType.Male, SocketType.Male);
            var (blockD, socketsD) = SetupSockets(SocketType.Female, SocketType.Female);
            
            socketsA[1].Connect(socketsB[0]);
            socketsB[1].Connect(socketsC[0]);
            socketsC[1].Connect(socketsD[0]);
            socketsD[1].Connect(socketsA[0]);

            var group = new HashSet<Block> {blockB, blockD};
            var result = blockA.GetComponent<BlockGroup>().SplitBy(group);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(group, result[0]);
            Assert.AreEqual(blockA, result[1].First());
            Assert.AreEqual(blockC, result[2].First());
            
            Assert.AreEqual(blockA.Connections.Count, 0);
            Assert.AreEqual(blockB.Connections.Count, 0);
            Assert.AreEqual(blockC.Connections.Count, 0);
            Assert.AreEqual(blockD.Connections.Count, 0);
        }
        
        [Test]
        public void SplitBreakCircle()
        {
            var (blockA, socketsA) = SetupSockets(SocketType.Male, SocketType.Male);
            var (blockB, socketsB) = SetupSockets(SocketType.Female, SocketType.Female);
            var (blockC, socketsC) = SetupSockets(SocketType.Male, SocketType.Male);
            var (blockD, socketsD) = SetupSockets(SocketType.Female, SocketType.Female);
            
            socketsA[1].Connect(socketsB[0]);
            socketsB[1].Connect(socketsC[0]);
            socketsC[1].Connect(socketsD[0]);
            socketsD[1].Connect(socketsA[0]);

            var group = new HashSet<Block> {blockB};
            var result = blockA.GetComponent<BlockGroup>().SplitBy(group);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(group, result[0]);
            CollectionAssert.AreEquivalent(new List<Block>{blockA, blockC, blockD}, result[1]);
            Assert.AreEqual(blockA.Connections.Count, 1);
            Assert.AreEqual(blockB.Connections.Count, 0);
            Assert.AreEqual(blockC.Connections.Count, 1);
            Assert.AreEqual(blockD.Connections.Count, 2);
        }
        
        [Test]
        public void SplitCenter()
        {
            var (blockA, socketsA) = SetupSockets(SocketType.Male, SocketType.Male, SocketType.Male);
            var (blockB, socketsB) = SetupSockets(SocketType.Female);
            var (blockC, socketsC) = SetupSockets(SocketType.Female);
            var (blockD, socketsD) = SetupSockets(SocketType.Female);

            socketsA[0].Connect(socketsB[0]);
            socketsA[1].Connect(socketsC[0]);
            socketsA[2].Connect(socketsD[0]);

            var group = new HashSet<Block> {blockA};
            var result = blockA.GetComponent<BlockGroup>().SplitBy(group);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(group, result[0]);
            Assert.AreEqual(blockB, result[1].First());
            Assert.AreEqual(blockC, result[2].First());
            Assert.AreEqual(blockD, result[3].First());
            Assert.AreEqual(blockA.Connections.Count, 0);
            Assert.AreEqual(blockB.Connections.Count, 0);
            Assert.AreEqual(blockC.Connections.Count, 0);
            Assert.AreEqual(blockD.Connections.Count, 0);
        }
    }
}