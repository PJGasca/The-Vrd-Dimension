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

        public void OnEnable()
        {
            // Find all the manipulatable objects in the scene
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Manipulatable");

            

        }
    }
}

