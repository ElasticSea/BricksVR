using _Framework.Scripts.Extensions;
using UnityEngine;

namespace _Project.Scripts
{
    public class SimplePlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 2;
        private void Update()
        {
            transform.position += Time.deltaTime * speed * OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).ToXZ();
        }
    }
}