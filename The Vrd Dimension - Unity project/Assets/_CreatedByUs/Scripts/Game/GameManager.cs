using Assets.Scripts.Objects;
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

        public HashSet<GameObject> objects;

        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameObject tetraSizeWarning;

        [SerializeField]
        private int maxOrderAgents;

        private int liveOrderAgents;

        private bool recalculateEntropy = false;

        public float EntropyPercentage
        {
            get; private set;
        }

        public void OnEnable()
        {
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

            if(!IsPowerOfFour(maxTetras))
            {
                tetraSizeWarning.GetComponent<TextMeshPro>().text = "WARNING! TOTAL TETRA SIZE IS CURRENTLY " + maxTetras + ". THIS IS NOT A POWER OF FOUR! THIS LEVEL MAY NOT BE COMPLETABLE!";
                tetraSizeWarning.SetActive(true);
            }

            maxObjects = maxTetras;

            StartCoroutine(AgentSpawner());
        }

        public void FixedUpdate()
        {
            if(recalculateEntropy)
            {
                int maxRange = maxObjects - minObjects;
                int totalInRange = objects.Count - minObjects;
                EntropyPercentage = totalInRange / (maxRange / 100);
                recalculateEntropy = false;
                Debug.Log("Entropy percentage = " + EntropyPercentage);
            }
        }

        private bool IsPowerOfFour(int n)
        {
            int count = 0;

            /*Check if there is only one bit
            set in n*/
            int x = n & (n - 1);

            if (n > 0 && x == 0)
            {
                /* count 0 bits before set bit */
                while (n > 1)
                {
                    n >>= 1;
                    count += 1;
                }

                /*If count is even then return 
                true else false*/
                return count % 2 == 0;
            }

            /* If there are more than 1 bit set
            then n is not a power of 4*/
            return false;
        }

        public void OnObjectAdded(GameObject obj)
        {
            objects.Add(obj);
            recalculateEntropy = true;
        }

        public void OnObjectRemoved(GameObject obj)
        {
            objects.Remove(obj);
            recalculateEntropy = true;
        }

        private IEnumerable AgentSpawner()
        {
            while(true)
            {
                yield return new WaitForSeconds(Random.Range(1f, 3f));
            }
        }
    }
}

