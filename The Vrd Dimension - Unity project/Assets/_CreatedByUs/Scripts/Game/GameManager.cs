using Assets.Scripts.Enemies;
using Assets.Scripts.Objects;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        private int maxObjects;

        private int minObjects = 1; // Assume all tetras can be merged into 1

        public HashSet<GameObject> objects = new HashSet<GameObject>();

        public static GameManager Instance { get; private set; }

        [SerializeField]
        private float agentSpawnRadius;

        [SerializeField]
        private float minAgentSpawnHeight;

        [SerializeField]
        private float maxAgentSpawnHeight;

        [SerializeField]
        private GameObject tetraSizeWarning;

        [SerializeField]
        private int maxOrderAgents;

        [SerializeField]
        private int maxChaosAgents;

        [SerializeField]
        [Tooltip("A higher value increases the likelihood of agents being spawned in any given attempt.")]
        private float chaosAgentSpawnLikelihoodScale;

        [SerializeField]
        [Tooltip("A higher value increases the likelihood of agents being spawned in any given attempt.")]
        private float orderAgentSpawnLikelihoodScale;

        [SerializeField]
        [Tooltip("The minimum amount of time between randomized rolls to decide whether or not to spawn an agent.")]
        private float minimumSpawnAttemptTime;

        [SerializeField]
        [Tooltip("The maximum amount of time between randomized rolls to decide whether or not to spawn an agent.")]
        private float maximumSpawnAttemptTime;

        [SerializeField]
        [Tooltip("Distance from start point that an item has to be before it is considered displaced by order agents.")]
        private float displacementRange;

        private int liveOrderAgents;

        private int liveChaosAgents;

        private bool recalculateEntropy = false;
        private Coroutine agentSpawnCoroutine;

        public float EntropyPercentage
        {
            get; private set;
        }

        public void Awake()
        {
            Instance = this;
        }

        public void OnEnable()
        {
            EndChecker.OnGameWin += StopAgentSpawning;

            // Find all the manipulatable objects in the scene
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Manipulatable");

            int maxTetras = 0;
            foreach(GameObject obj in objects)
            {
                // Assume it's a tetra for now
                ObjectSize size = obj.GetComponent<ObjectSize>();
                if(size == null)
                {
                    Debug.LogWarning("Found a tetra that didn't have a size component on it.  That can't be right!");
                }
                else
                {
                    maxTetras += size.Size;
                }
            }

            /*
            if(!IsPowerOfFour(maxTetras))
            {
                tetraSizeWarning.GetComponent<TextMeshPro>().text = "WARNING! TOTAL TETRA SIZE IS CURRENTLY " + maxTetras + ". THIS IS NOT A POWER OF FOUR! THIS LEVEL MAY NOT BE COMPLETABLE!";
                tetraSizeWarning.SetActive(true);
            }*/

            maxObjects = maxTetras;

            agentSpawnCoroutine = StartCoroutine(AgentSpawner());
        }

        void OnDisable () {
            EndChecker.OnGameWin -= StopAgentSpawning;
        }

        public void FixedUpdate()
        {
            if(recalculateEntropy)
            {
                int maxRange = maxObjects - minObjects;
                int totalInRange = objects.Count - minObjects;
                EntropyPercentage = totalInRange / (maxRange / 100f);
                recalculateEntropy = false;
                Debug.Log("Entropy percentage = " + EntropyPercentage);
            }
        }
        /*
        private bool IsPowerOfFour(int n)
        {
            int count = 0;

            // Check if there is only one bit set in n
            int x = n & (n - 1);

            if (n > 0 && x == 0)
            {
                // count 0 bits before set bit 
                while (n > 1)
                {
                    n >>= 1;
                    count += 1;
                }

                // If count is even then return true else false
                return count % 2 == 0;
            }

            // If there are more than 1 bit set then n is not a power of 4
            return false;
        }*/

        public void OnObjectAdded(GameObject obj)
        {
            objects.Add(obj);
            //Debug.Log("Object added to game manager");
            recalculateEntropy = true;
        }

        public void OnObjectRemoved(GameObject obj)
        {
            objects.Remove(obj);
            recalculateEntropy = true;
        }

        private IEnumerator AgentSpawner()
        {
            while(true)
            {
                yield return new WaitForSeconds(Random.Range(minimumSpawnAttemptTime, maximumSpawnAttemptTime));

                Debug.Log("Spawn check.  Entropy: " + EntropyPercentage + " Live chaos: " + liveChaosAgents + " Live order: " + liveOrderAgents);

                bool spawned = false;
                if (liveOrderAgents < maxOrderAgents && Random.Range(0, 100) < (EntropyPercentage * orderAgentSpawnLikelihoodScale) && GetDisplacedUntargetedTetra()!=null)
                {
                    SpawnOrderAgent();
                    spawned = true;
                }

                if (liveChaosAgents < maxChaosAgents && Random.Range(0, 100) < ((100-EntropyPercentage) * chaosAgentSpawnLikelihoodScale) && GetBreakableTetra() != null)
                {
                    if(spawned)
                    {
                        // We could potentially spawn a chaos and order agent in the same attempt, but it might look very obvious if both pop into existence at 
                        // exactly the same time so wait just a little before spawning.
                        yield return new WaitForSeconds(0.5f);
                    }

                    SpawnChaosAgent();
                }
            }
        }

        private void SpawnOrderAgent()
        {
            Vector3 pointOnSphere = Random.onUnitSphere * agentSpawnRadius;
            Vector3 spawnPoint = new Vector3(pointOnSphere.x, Random.Range(minAgentSpawnHeight, maxAgentSpawnHeight), pointOnSphere.z);
            GameObject agent = ObjectPool.Instance.GetObjectForType("AgentOfOrder");
            agent.transform.position = spawnPoint;
            liveOrderAgents++;
            agent.GetComponent<OrderAgentSpawning>().Spawn();
            agent.GetComponent<OrderAgentDying>().OnDeath += OnOrderAgentDeath;
        }

        private void SpawnChaosAgent()
        {
            Debug.Log("Spawning chaos agent");
            Vector3 pointOnSphere = Random.onUnitSphere * agentSpawnRadius;
            Vector3 spawnPoint = new Vector3(pointOnSphere.x, Random.Range(minAgentSpawnHeight, maxAgentSpawnHeight), pointOnSphere.z);
            GameObject agent = ObjectPool.Instance.GetObjectForType("AgentOfChaos");
            agent.transform.position = spawnPoint;
            liveChaosAgents++;
            agent.GetComponent<ChaosAgentSpawning>().Spawn();
            agent.GetComponent<ChaosAgentDying>().OnDeath += OnChaosAgentDeath;
        }

        private void OnOrderAgentDeath(GameObject agent)
        {
            agent.GetComponent<OrderAgentDying>().OnDeath -= OnOrderAgentDeath;
            liveOrderAgents--;
        }

        private void OnChaosAgentDeath(GameObject agent)
        {
            agent.GetComponent<ChaosAgentDying>().OnDeath -= OnChaosAgentDeath;
            liveChaosAgents--;
        }

        public MergableObject GetDisplacedUntargetedTetra()
        {
            MergableObject displaced = null;
            foreach (MergableObject tetra in MergableObject.All)
            {
                if (IsDisplaced(tetra) && !tetra.targetedByAgent && !tetra.GetComponent<Grabbable>().IsGrabbed)
                {
                    displaced = tetra;
                    break;
                }
            }
            return displaced;
        }

        public MergableObject GetBreakableTetra()
        {
            MergableObject breakable = null;
            foreach (MergableObject tetra in MergableObject.All)
            {
                if (tetra.gameObject.GetComponent<ObjectSize>().Size > 1 && !tetra.targetedByAgent && !tetra.GetComponent<Grabbable>().IsGrabbed)
                {
                    breakable = tetra;
                    break;
                }
            }
            return breakable;
        }

        private bool IsDisplaced(MergableObject tetra)
        {
            return Vector3.Distance(tetra.transform.position, tetra.SpawnPosition) > displacementRange;
        }

        private void StopAgentSpawning () {
            StopCoroutine (agentSpawnCoroutine);
            agentSpawnCoroutine = null;
        }
    }
}

