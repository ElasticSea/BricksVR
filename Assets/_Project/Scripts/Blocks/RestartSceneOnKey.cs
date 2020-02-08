using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Blocks
{
    public class RestartSceneOnKey : MonoBehaviour
    {
        [SerializeField] private OVRInput.RawButton button = OVRInput.RawButton.Start;

        private void Update()
        {
            if (OVRInput.GetDown(button))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}