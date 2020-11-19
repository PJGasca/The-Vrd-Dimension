using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Utility
{
    public class PoolOnParticleEnd : MonoBehaviour
    {
        private ParticleSystem particles;

        public void Start()
        {
            particles = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!particles.isEmitting)
            {
                ObjectPool.Instance.PoolObject(gameObject);
            }
        }
    }
}
