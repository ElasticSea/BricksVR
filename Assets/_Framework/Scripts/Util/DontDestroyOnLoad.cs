using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() => DontDestroyOnLoad(gameObject);
    }
}