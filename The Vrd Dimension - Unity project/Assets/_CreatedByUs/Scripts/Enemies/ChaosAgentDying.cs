using Assets.Scripts.Objects;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(ChaosAgentSpawning))]
    [RequireComponent(typeof(AgentScaler))]
    [RequireComponent(typeof(Collider))]
    public class ChaosAgentDying : MonoBehaviour
    {
        private ChaosAgentSpawning spawningBehaviour;

        public System.Action<GameObject> OnDeath;

        private AudioSource audioSource;

        [SerializeField]
        private float shrinkTime;
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
            spawningBehaviour = GetComponent<ChaosAgentSpawning>();
            audioSource.clip = SoundEffectClips.instance.orderDeath[Random.Range(0, SoundEffectClips.instance.orderDeath.Count)];
            audioSource.PlayOneShot(audioSource.clip, 3);
            StartCoroutine(ShrinkRoutine());
        }

       private IEnumerator ShrinkRoutine()
       {
            AgentScaler scaler = GetComponent<AgentScaler>();
            scaler.SetScale(Vector3.zero, shrinkTime);

            yield return new WaitForSeconds(audioSource.clip.length);

            // Reset to default size
            spawningBehaviour.enabled = true;
            scaler.ResetScale();
            //Debug.Log("Agent dying");
            if (OnDeath != null)
            {
                OnDeath(gameObject);
            }
            else
            {
                Debug.LogWarning("No delegate for chaos agent death");
            }
            ObjectPool.Instance.PoolObject(gameObject);
            this.enabled = false;
        }
    }
}
