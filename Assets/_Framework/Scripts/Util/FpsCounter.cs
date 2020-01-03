using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private int frameBufferCount;

        private float[] frameBuffer;
        private int currentFrame;

        private void Awake()
        {
            frameBuffer = new float[frameBufferCount];
        }

        private void Update()
        {
            frameBuffer[currentFrame++ % frameBuffer.Length] = Time.timeScale / Time.deltaTime;
        }

        public float Fps
        {
            get
            {
                float acu = 0;
                for (var i = 0; i < frameBuffer.Length; i++)
                {
                    acu += frameBuffer[i];
                }
                return acu / frameBuffer.Length;
            }
        }
    }
}