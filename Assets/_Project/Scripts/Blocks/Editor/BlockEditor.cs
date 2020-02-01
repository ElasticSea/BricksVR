using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Blocks.Editor
{
    [CustomEditor(typeof(Block)), CanEditMultipleObjects]
    public class BlockEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var blocks = targets.Cast<Block>();
            if (GUILayout.Button("Split"))
            {
                var blockGroups = blocks.GroupBy(l => l.GetComponentInParent<BlockGroup>());
                foreach (var blockGroup in blockGroups)
                {
                    BlockFactory.DisconnectChunk(blockGroup.Key, blockGroup.ToList());
                }
            }
        }
    }
}