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

        [SerializeField]
        private PlayerBeam beam;

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

            if(repel)
            {
                beam.Mode = PlayerBeam.BeamMode.BOTH;
            }
            else
            {
                beam.Mode = PlayerBeam.BeamMode.ATTRACT;
            }
        }

        public void OnAttractEnd()
        {
            attract = false;

            if(!repel)
            {
                if (attractTime <= CLICK_TIME)
                {
                    // Fast attract
                }
                beam.Mode = PlayerBeam.BeamMode.OFF;
            }
            else
            {
                beam.Mode = PlayerBeam.BeamMode.REPEL;
            }

            attractTime = 0f;
        }

        public void OnRepelStart()
        {
            repel = true;
            if (attract)
            {
                beam.Mode = PlayerBeam.BeamMode.BOTH;
            }
            else
            {
                beam.Mode = PlayerBeam.BeamMode.REPEL;
            }
        }

        public void OnRepelEnd()
        {
            repel = false;
            repelTime = 0f;

            if(!attract)
            {
                beam.Mode = PlayerBeam.BeamMode.OFF;
            }
            else
            {
                beam.Mode = PlayerBeam.BeamMode.ATTRACT;
            }
        }
    }
}

