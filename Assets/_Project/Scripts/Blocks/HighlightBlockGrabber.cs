using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Blocks
{
    public class HighlightBlockGrabber : MonoBehaviour
    {
        private HashSet<Block> previousCandidates = new HashSet<Block>();
        private HashSet<Block> currectCandidates = new HashSet<Block>();

        // OnTriggerEnter/Exit does not catch instantiate and destroy events on objects with colliders
        private void OnTriggerStay(Collider other)
        {
            var block = other.GetComponent<Block>();
            if (block)
            {
                currectCandidates.Add(block);
            }
        }

        private void Update()
        {
            foreach (var previousCandidate in previousCandidates.ToList())
            {
                // Block removed
                if (currectCandidates.Contains(previousCandidate) == false)
                {
                    previousCandidates.Remove(previousCandidate);
                    Remove(previousCandidate);
                }
            }

            foreach (var currectCandidate in currectCandidates.ToList())
            {
                // Block added
                if (previousCandidates.Contains(currectCandidate) == false)
                {
                    previousCandidates.Add(currectCandidate);
                    Add(currectCandidate);
                }
            }
            
            currectCandidates.Clear();
        }

        private void Add(Block grabbable)
        {
            grabbable.gameObject.AddComponent<Outline>().OutlineWidth = 10;
        }

        private void Remove(Block grabbable)
        {
            Destroy(grabbable.GetComponent<Outline>());
        }
    }
}