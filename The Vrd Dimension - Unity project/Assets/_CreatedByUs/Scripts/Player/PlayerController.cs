using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        private float attractTime = 0;
        private float repelTime = 0;

        private bool attract = false;
        private bool repel = false;

        private static readonly float CLICK_TIME = 0.25f;

        // Use this for initialization
        void Start()
        {

        }

        void FixedUpdate()
        {
            if(attract)
            {
                attractTime += Time.fixedDeltaTime;
            }

            if(repel)
            {
                repelTime += Time.fixedDeltaTime;
            }


        }

        public void OnAttractStart()
        {
            attract = true;
        }

        public void OnAttractEnd()
        {
            attract = false;

            if(attractTime <= CLICK_TIME)
            {
                // Fast attract
            }

            attractTime = 0f;
        }

        public void OnRepelStart()
        {
            repel = true;
        }

        public void OnRepelEnd()
        {
            repel = false;
            repelTime = 0f;
        }
    }
}

