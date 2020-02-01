using System.Collections.Generic;
using System.Linq;
using _Project.Scripts;
using NUnit.Framework;
using UnityEngine;

namespace _Project.Tests.Scripts.Editor
{
    public class ChunkLinkTest
    {
        [Test]
        public void SplitNone()
        {
            var go = new GameObject();

            var linkA = go.AddComponent<ChunkLink>();
            
            var group = new HashSet<ChunkLink> {};
            var result = linkA.SplitBy(group);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(linkA, result[1].First());
            Assert.AreEqual(linkA.Connections.Count, 0);
        }
        
        [Test]
        public void SplitSame()
        {
            var go = new GameObject();

            var linkA = go.AddComponent<ChunkLink>();
            
            var group = new HashSet<ChunkLink> {linkA};
            var result = linkA.SplitBy(group);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(group, result[0]);
            Assert.AreEqual(linkA.Connections.Count, 0);
        }
        
        [Test]
        public void SplitCircleInHalf()
        {
            var go = new GameObject();

            var linkA = go.AddComponent<ChunkLink>();
            var linkB = go.AddComponent<ChunkLink>();
            var linkC = go.AddComponent<ChunkLink>();
            var linkD = go.AddComponent<ChunkLink>();
            
            linkA.Connect(linkB);
            linkB.Connect(linkC);
            linkC.Connect(linkD);
            linkD.Connect(linkA);

            var group = new HashSet<ChunkLink> {linkB, linkD};
            var result = linkA.SplitBy(group);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(group, result[0]);
            Assert.AreEqual(linkA, result[1].First());
            Assert.AreEqual(linkC, result[2].First());
            
            Assert.AreEqual(linkA.Connections.Count, 0);
            Assert.AreEqual(linkB.Connections.Count, 0);
            Assert.AreEqual(linkC.Connections.Count, 0);
            Assert.AreEqual(linkD.Connections.Count, 0);
        }
        
        [Test]
        public void SplitBreakCircle()
        {
            var go = new GameObject();

            var linkA = go.AddComponent<ChunkLink>();
            var linkB = go.AddComponent<ChunkLink>();
            var linkC = go.AddComponent<ChunkLink>();
            var linkD = go.AddComponent<ChunkLink>();
            
            linkA.Connect(linkB);
            linkB.Connect(linkC);
            linkC.Connect(linkD);
            linkD.Connect(linkA);

            var group = new HashSet<ChunkLink> {linkB};
            var result = linkA.SplitBy(group);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(group, result[0]);
            CollectionAssert.AreEquivalent(new List<ChunkLink>{linkA, linkC, linkD}, result[1]);
            Assert.AreEqual(linkA.Connections.Count, 1);
            Assert.AreEqual(linkB.Connections.Count, 0);
            Assert.AreEqual(linkC.Connections.Count, 1);
            Assert.AreEqual(linkD.Connections.Count, 2);
        }
        
        [Test]
        public void SplitCenter()
        {
            var go = new GameObject();

            var linkA = go.AddComponent<ChunkLink>();
            var linkB = go.AddComponent<ChunkLink>();
            var linkC = go.AddComponent<ChunkLink>();
            var linkD = go.AddComponent<ChunkLink>();
            
            linkA.Connect(linkB);
            linkA.Connect(linkC);
            linkA.Connect(linkD);

            var group = new HashSet<ChunkLink> {linkA};
            var result = linkA.SplitBy(group);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(group, result[0]);
            Assert.AreEqual(linkB, result[1].First());
            Assert.AreEqual(linkC, result[2].First());
            Assert.AreEqual(linkD, result[3].First());
            Assert.AreEqual(linkA.Connections.Count, 0);
            Assert.AreEqual(linkB.Connections.Count, 0);
            Assert.AreEqual(linkC.Connections.Count, 0);
            Assert.AreEqual(linkD.Connections.Count, 0);
        }
    }
}