using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class AltF4FreezeWorkaround : Singleton<AltF4FreezeWorkaround>
    {
        // https://issuetracker.unity3d.com/issues/build-crashes-while-closing-it-with-alt-plus-f4-when-using-net4-dot-6
        private void OnApplicationQuit()
        {
            if (enabled && Application.platform == RuntimePlatform.WindowsPlayer)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        protected override void OnSingletonAwake()
        {
        }
    }
}