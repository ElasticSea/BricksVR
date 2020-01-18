using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    [CustomEditor(typeof(Block))]
    public class BlockEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var block = (Block) target;
            if (GUILayout.Button("Begin Snap"))
            {
                block.BeginSnap();
            }

            if (GUILayout.Button("End Snap"))
            {
                block.EndSnap();
            }
        }
    }
}