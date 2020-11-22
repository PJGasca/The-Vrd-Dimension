using Assets.Scripts.Objects;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(OrderAgentSpawning))]
    [RequireComponent(typeof(AgentScaler))]
    public class OrderAgentDying : MonoBehaviour
    {
        private OrderAgentSpawning spawningBehaviour;

        public System.Action<GameObject> OnDeath;

        [SerializeField]
        private float shrinkTime;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }


        private void OnEnable()
        {
            GetComponent<Collider>().enabled = false;
            spawningBehaviour = GetComponent<OrderAgentSpawning>();
            audioSource.PlayOneShot(SoundEffectClips.instance.orderDeath[Random.Range(0, SoundEffectClips.instance.orderDeath.Count)]);
            StartCoroutine(ShrinkRoutine());
        }

       private IEnumerator ShrinkRoutine()
       {
            AgentScaler scaler = GetComponent<AgentScaler>();
            scaler.SetScale(Vector3.zero, shrinkTime);

            yield return new WaitForSeconds(shrinkTime);

            // Reset to default size
            spawningBehaviour.enabled = true;
            scaler.ResetScale();
            Debug.Log("Agent dying");
            if (OnDeath != null)
            {
                OnDeath(gameObject);
            }
            else
            {
                Debug.LogWarning("No delegate for order agent death");
            }
            ObjectPool.Instance.PoolObject(gameObject);
            this.enabled = false;
        }
    }
}
