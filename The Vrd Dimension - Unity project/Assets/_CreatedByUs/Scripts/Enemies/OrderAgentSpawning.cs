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
        
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Spawn()
        {
            GetComponent<Collider>().enabled = false;

            transform.localScale = Vector3.zero;
            seekingBehaviour = GetComponent<OrderAgentSeeking>();
            audioSource.PlayOneShot(SoundEffectClips.instance.orderSpawn[Random.Range(0, SoundEffectClips.instance.orderSpawn.Count)]);
            StartCoroutine(SpawnInRoutine());
        }

        private IEnumerator SpawnInRoutine()
        {
            GetComponent<AgentScaler>().ScaleToInitial(spawnInTime);
            yield return new WaitForSeconds(spawnInTime);
            seekingBehaviour.enabled = true;
            this.enabled = false;
        }
    }
}
