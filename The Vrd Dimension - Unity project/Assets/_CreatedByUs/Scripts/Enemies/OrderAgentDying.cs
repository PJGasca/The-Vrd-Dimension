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
    public class OrderAgentDying : MonoBehaviour
    {
        private OrderAgentSeeking seekingBehaviour;

        [SerializeField]
        private float shrinkTime;

        private void OnEnable()
        {
            StartCoroutine(ShrinkRoutine());
            seekingBehaviour = GetComponent<OrderAgentSeeking>();
        }

       private IEnumerator ShrinkRoutine()
       {
            AgentScaler scaler = GetComponent<AgentScaler>();
            scaler.SetScale(Vector3.zero, shrinkTime);

            yield return new WaitForSeconds(shrinkTime);

            // Reset to default size
            seekingBehaviour.enabled = true;
            this.enabled = false;
            scaler.ResetScale();
            ObjectPool.Instance.PoolObject(gameObject);
       }
    }
}
