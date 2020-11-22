﻿using Assets.Scripts.Objects;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AgentScaler))]
    public class ChaosAgentSpawning : MonoBehaviour
    {
        private ChaosAgentSeeking seekingBehaviour;

        [SerializeField]
        private float spawnInTime;

        public void Spawn()
        {
            transform.localScale = Vector3.zero;
            seekingBehaviour = GetComponent<ChaosAgentSeeking>();
            Collider[] colliders = GetComponents<Collider>();
            foreach(Collider collider in colliders)
            {
                collider.enabled = false;
            }
            
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
