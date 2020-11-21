using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        private int maxObjects;

        private int minObjects;

        private HashSet<GameObject> objects;

        public static GameManager Instance { get; private set; }

        [SerializeField]
        [Tooltip("The number of the smallest size tetras that make up the largest possible.")]
        private int maxTetraSize;

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
        }
    }
}

