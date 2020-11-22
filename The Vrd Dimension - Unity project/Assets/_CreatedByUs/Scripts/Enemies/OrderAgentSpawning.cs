using Assets.Scripts.Objects;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(OrderAgentSeeking))]
    [RequireComponent(typeof(AgentScaler))]
    public class OrderAgentSpawning : MonoBehaviour
    {
        private OrderAgentSeeking seekingBehaviour;

        [SerializeField]
        private float spawnInTime;

        public void Spawn()
        {
            seekingBehaviour = GetComponent<OrderAgentSeeking>();
            StartCoroutine(SpawnInRoutine());
        }

        private IEnumerator SpawnInRoutine()
        {
            yield return new WaitForSeconds(spawnInTime);
            seekingBehaviour.enabled = true;
        }

      /* private IEnumerator ShrinkRoutine()
       {
            AgentScaler scaler = GetComponent<AgentScaler>();
            scaler.SetScale(Vector3.zero, shrinkTime);

            yield return new WaitForSeconds(shrinkTime);

            // Reset to default size
            seekingBehaviour.enabled = true;
            scaler.ResetScale();
            Debug.Log("Agent dying");
            if (OnDeath != null)
            {
                OnDeath(gameObject);
            }
            else
            {
                Debug.Log("No delegate");
            }
            ObjectPool.Instance.PoolObject(gameObject);
            this.enabled = false;
        }*/
    }
}
