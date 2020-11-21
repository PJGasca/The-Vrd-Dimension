using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(PlayerBeam))]
    public class PlayerBeamController : MonoBehaviour
    {
        private float attractTime = 0;
        private float repelTime = 0;

        private bool attract = false;
        private bool repel = false;

        private static readonly float CLICK_TIME = 0.25f;

        private PlayerBeam beam;

        [SerializeField]
        private float force;

        public void Awake()
        {
            beam = GetComponent<PlayerBeam>();
        }

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

            if(attract ^ repel)    // XOR
            {
                HashSet<GameObject> objects = beam.ObjectsInBeam;
                foreach(GameObject toAffect in objects)
                {
                    ApplyForceToObject(toAffect);
                }
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

        private void ApplyForceToObject(GameObject toApply)
        {
            Vector3 forceToApply = transform.forward * force * Time.fixedDeltaTime;

            if(attract)
            {
                forceToApply *= -1;
            }

            toApply.GetComponent<Rigidbody>().AddForce(forceToApply);
        }
    }
}

