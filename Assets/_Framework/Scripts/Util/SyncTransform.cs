using System;
using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class SyncTransform : MonoBehaviour
    {
        public enum SyncMode
        {
            Update, FixedUpdate, LateUpdate
        }
        
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private SyncMode mode = SyncMode.Update;
        public bool syncPosition;
        public bool syncRotation;
        public bool syncLocalRotation;
        public bool syncLocalRotationX = true;
        public bool syncLocalRotationY = true;
        public bool syncLocalRotationZ = true;
        public bool syncScale;

        public Transform Target
        {
            get => target;
            set
            {
                target = value;
                Sync();
            }
        }

        public Vector3 PositionOffset
        {
            get => positionOffset;
            set
            {
                positionOffset = value;
                Sync();
            }
        }

        public Vector3 RotationOffset
        {
            get => rotationOffset;
            set
            {
                rotationOffset = value;
                Sync();
            }
        }

        public SyncMode Mode
        {
            get => mode;
            set
            {
                mode = value;
                Sync();
            }
        }

        private void Update()
        {
            if(mode == SyncMode.Update) Sync();
        }

        private void FixedUpdate()
        {
            if(mode == SyncMode.FixedUpdate) Sync();
        }

        private void LateUpdate()
        {
            if(mode == SyncMode.LateUpdate) Sync();
        }

        private void Sync()
        {
            if (Target == false) return;

            if (syncPosition)
                transform.position = Target.position + PositionOffset;

            if (syncRotation)
                transform.rotation = Quaternion.Euler(Target.rotation.eulerAngles + rotationOffset);

            if (syncLocalRotation)
            {
                var syncRotation = Vector3.zero;
                var targetLocalRotation = (Target.localRotation.eulerAngles + rotationOffset);

                if (syncLocalRotationX) syncRotation.x = targetLocalRotation.x;
                if (syncLocalRotationY) syncRotation.y = targetLocalRotation.y;
                if (syncLocalRotationZ) syncRotation.z = targetLocalRotation.z;
                
                transform.localRotation = Quaternion.Euler(syncRotation);
            }

            if (syncScale)
                transform.localScale = Target.lossyScale;
        }
    }
}