using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class BoundedParticles : MonoBehaviour
    {
        [SerializeField] private BoxCollider box;
        [SerializeField] private ParticleSystem particleSystem;
    
        private void Update()
        {
            var emittedParticles = new ParticleSystem.Particle[particleSystem.particleCount];
            particleSystem.GetParticles(emittedParticles);

            for (var i = 0; i < emittedParticles.Length; i++)
            {
                var localPos = box.transform.InverseTransformPoint(emittedParticles[i].position);
                var x = (localPos.x + (1.5f * box.size.x)) % box.size.x - box.size.x / 2f;
                var y = (localPos.y + (1.5f * box.size.y)) % box.size.y - box.size.y / 2f;
                var z = (localPos.z + (1.5f * box.size.z)) % box.size.z - box.size.z / 2f;
                emittedParticles[i].position = box.transform.TransformPoint(new Vector3(x, y, z));
            }
        
            particleSystem.SetParticles(emittedParticles);
        }
    }
}
