using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class MobilePlatformFix : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}