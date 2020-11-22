using Assets.Scripts.Objects;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(ChaosAgentSpawning))]
    [RequireComponent(typeof(AgentScaler))]
    public class ChaosAgentDying : MonoBehaviour
    {
        private ChaosAgentSpawning spawningBehaviour;

        public System.Action<GameObject> OnDeath;

        [SerializeField]
        private float shrinkTime;

        private void OnEnable()
        {
            spawningBehaviour = GetComponent<ChaosAgentSpawning>();
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
            //Debug.Log("Agent dying");
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
        }
    }
}
