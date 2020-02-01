using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Blocks.Editor
{
    [CustomEditor(typeof(BlockGroup))]
    public class BlockGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var block = (BlockGroup) target;
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Begin Snap")) block.BeginSnap();
            if (GUILayout.Button("End Snap")) block.EndSnap();
            GUILayout.EndHorizontal();
        }
    }
}