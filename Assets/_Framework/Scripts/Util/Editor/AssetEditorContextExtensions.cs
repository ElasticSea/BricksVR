using UnityEditor;
using UnityEngine;

namespace _Framework.Scripts.Util.Editor
{
    public static class AssetEditorContextExtensions
    {
        [MenuItem("Assets/Copy GUID to clipboard")]
        private static void LoadAdditiveScene()
        {
            var asset = Selection.activeObject;
            var path  = AssetDatabase.GetAssetPath(asset);
            var guid = AssetDatabase.AssetPathToGUID(path);

            Debug.Log(asset + " [" + guid + "] copied to clipboard.");
            EditorGUIUtility.systemCopyBuffer = guid;
        }
    }
}