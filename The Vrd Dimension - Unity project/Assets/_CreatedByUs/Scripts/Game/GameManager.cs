using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        private float entropy;

        private float minEntropy;

        private float maxEntropy;

        private float entropyPercentage;

        public static GameManager Instance { get; private set; }

        public void OnEnable()
        {
            // Find all the manipulatable objects in the scene
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Manipulatable");

            // Work out min/max entropies

        }
    }
}

