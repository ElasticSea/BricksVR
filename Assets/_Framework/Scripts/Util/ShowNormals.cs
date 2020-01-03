using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class ShowNormals : MonoBehaviour
    {
        [SerializeField] private MeshFilter mf;
        [SerializeField] private Color startColor = Color.blue;
        [SerializeField] private Color endColor = Color.red;

        private void OnDrawGizmosSelected()
        {
            var vertices = mf.mesh.vertices;
            var normals = mf.mesh.normals;

            for (var i = 0; i < normals.Length; i++)
            {
                Gizmos.color = Color.Lerp(startColor, endColor, (float) i / (normals.Length - 1));

                var worldPos = transform.TransformPoint(vertices[i]);
                var worldNormal = transform.TransformVector(normals[i]);
                Gizmos.DrawLine(worldPos, worldPos + worldNormal);
            }
        }
    }
}