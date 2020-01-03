using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class DisableFrustumCulling : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera camera;

        private void OnPreCull()
        {
            camera.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
                                   Matrix4x4.Translate(Vector3.forward * -99999 / 2f) *
                                   camera.worldToCameraMatrix;
        }

        private void OnDisable()
        {
            camera.ResetCullingMatrix();
        }
    }
}