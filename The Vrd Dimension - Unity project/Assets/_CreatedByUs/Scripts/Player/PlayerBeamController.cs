using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Objects;
using Assets.Scripts.Enemies;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(PlayerBeam))]
    public class PlayerBeamController : MonoBehaviour
    {
        private float attractTime = 0;
        private float repelTime = 0;

        private bool attract = false;
        private bool repel = false;

        private static readonly float CLICK_TIME = 0.1f;

        private PlayerBeam beam;

        private Grabbable grabbedObject = null;

        [SerializeField]
        private float force;

        [SerializeField]
        private float repelBurstForce;

        [SerializeField]
        private Transform attachmentPoint;

        [SerializeField]
        private float agentHealRate;

        [SerializeField]
        private float agentDamageRate;

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
            if(grabbedObject == null)
            {
                if (attract)
                {
                    attractTime += Time.fixedDeltaTime;
                }

                if (repel)
                {
                    repelTime += Time.fixedDeltaTime;
                }

                if (attract ^ repel)    // XOR
                {
                    AffectObjectsInBeam();
                }
            }
        }

        private void AffectObjectsInBeam()
        {
            HashSet<GameObject> objects = beam.ObjectsInBeam;
            foreach (GameObject toAffect in objects)
            {
                if (toAffect.CompareTag("ChaosAgent"))
                {
                    HealthTracker health = toAffect.GetComponent<HealthTracker>();
                    if (attract)
                    {
                        health.InflictDamage(agentDamageRate * Time.fixedDeltaTime);
                    }
                    else
                    {
                        health.Heal(agentDamageRate * Time.fixedDeltaTime);
                    }
                }
                else if (toAffect.CompareTag("OrderAgent"))
                {
                    HealthTracker health = toAffect.GetComponent<HealthTracker>();
                    if (repel)
                    {
                        health.InflictDamage(agentDamageRate * Time.fixedDeltaTime);
                    }
                    else
                    {
                        health.Heal(agentDamageRate * Time.fixedDeltaTime);
                    }
                }
                else
                {
                    Grabbable grabbable = toAffect.GetComponent<Grabbable>();
                    if (grabbable != null && grabbable.IsGrabbed)
                    {
                        grabbable.Release();
                    }

                    ApplyForceToObject(toAffect);
                }
            }
        }

        public void OnAttractStart()
        {
            if(grabbedObject == null)
            {
                attract = true;

                if (repel)
                {
                    beam.Mode = PlayerBeam.BeamMode.BOTH;
                }
                else
                {
                    beam.Mode = PlayerBeam.BeamMode.ATTRACT;
                }
            }

        }

        public void OnAttractEnd()
        {
            if(grabbedObject == null)
            {
                attract = false;

                if (!repel)
                {
                    if (attractTime <= CLICK_TIME)
                    {
                        GrabClosestObject();
                    }
                    else
                    {
                        beam.Mode = PlayerBeam.BeamMode.NEUTRAL;
                    }
                }
                else
                {
                    beam.Mode = PlayerBeam.BeamMode.REPEL;
                }

                attractTime = 0f;
            }

        }

        public void OnRepelStart()
        {
            repel = true;
            if(grabbedObject!=null)
            {
                ReleaseGrabbedObject();
            }

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
            if(grabbedObject==null)
            {
                repel = false;

                if (!attract)
                {
                    if (repelTime <= CLICK_TIME)
                    {
                        // Fast repel
                        FastRepelClosestObject();
                    }

                    beam.Mode = PlayerBeam.BeamMode.NEUTRAL;
                }
                else
                {
                    beam.Mode = PlayerBeam.BeamMode.ATTRACT;
                }

                repelTime = 0f;
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

        private void GrabClosestObject()
        {
            GameObject closest = FindClosestObjectInBeam();
            if(closest!=null && closest.activeSelf)
            {
                Grabbable grabbable = closest.GetComponent<Grabbable>();
                if (grabbable != null)
                {
                    grabbable.Grab(attachmentPoint);
                    grabbedObject = grabbable;
                    beam.Mode = PlayerBeam.BeamMode.OFF;
                }
            }

        }

        private void ReleaseGrabbedObject()
        {
            if(grabbedObject!=null)
            {
                grabbedObject.Release();
                grabbedObject = null;
            }
        }

        private void FastRepelClosestObject()
        {
            GameObject closest = FindClosestObjectInBeam();
            if(closest != null)
            {
                Vector3 forceToApply = transform.forward * repelBurstForce;
                closest.GetComponent<Rigidbody>().AddForce(forceToApply);
            }
        }

        private GameObject FindClosestObjectInBeam()
        {
            GameObject closest = null;
            float closestDistance = 0;
            foreach(GameObject obj in beam.ObjectsInBeam)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (closest == null || distance < closestDistance)
                {
                    closest = obj;
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}

