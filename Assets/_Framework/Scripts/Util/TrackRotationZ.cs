using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class TrackRotationZ : MonoBehaviour
    {
        public float z;
        private float? lastRot;

        private void Update()
        {
            var cur = transform.localRotation.eulerAngles;
            if (lastRot.HasValue)
            {
                var curz = Mathf.Abs(cur.z - z);
                var diff = ((cur.z -360)%360);
                var difz = Mathf.Abs(diff - z);
                z = curz < difz ? cur.z : diff;
            }
            lastRot = z;
        }
    }
}