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

        private AudioSource[] audioSources;
        bool trailingOff;
        IEnumerator playAudio;
        Utility.SoundEffectClips sfx;

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

        [SerializeField]
        private float autoGrabRange;

        [SerializeField]
        private float maxGrabSize;

        public void Awake()
        {
            beam = GetComponent<PlayerBeam>();
            audioSources = GetComponents<AudioSource>();
        }

        // Use this for initialization
        void Start()
        {
            sfx = Utility.SoundEffectClips.instance;
            if (sfx != null)
            {
                if (sfx.holdAttract != null) audioSources[0].clip = sfx.holdAttract;
                if (sfx.holdRepel != null) audioSources[1].clip = sfx.holdRepel;
            }
        }

        void FixedUpdate()
        {
            if (grabbedObject == null)
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
                HealthTracker health = toAffect.GetComponent<HealthTracker>();
                if (health)
                {
                    health.InflictDamage(agentDamageRate * Time.deltaTime);
                }
                else
                {
                    Grabbable grabbable = toAffect.GetComponent<Grabbable>();
                    if (grabbable != null && grabbable.IsGrabbed)
                    {
                        grabbable.Release();
                    }
                    else if (attract && !repel && Vector3.Distance(toAffect.transform.position, attachmentPoint.transform.position) <= autoGrabRange)
                    {
                        GrabObject(toAffect);
                    }
                    else
                    {
                        ApplyForceToObject(toAffect);
                    }
                }
            }
        }

        public void OnAttractStart()
        {
            if (grabbedObject == null)
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

                if (playAudio != null) StopCoroutine(playAudio);
                playAudio = PlayLoopingAudio();
                StartCoroutine(playAudio);
            }
            else
            {
                ReleaseGrabbedObject();
                attractTime = CLICK_TIME + 1; // Make sure we don't accidentally grab it again
            }

        }

        public void OnAttractEnd()
        {
            if (grabbedObject == null)
            {
                attract = false;

                if (!repel)
                {
                    if (attractTime <= CLICK_TIME)
                    {
                        if (!GrabClosestObject())
                        {
                            beam.Mode = PlayerBeam.BeamMode.NEUTRAL;
                        }
                        audioSources[2].PlayOneShot(sfx.grab);
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
            if (grabbedObject != null)
            {
                ReleaseGrabbedObject();
                repelTime = CLICK_TIME + 1; // Don't pulse
            }

            if (attract)
            {
                beam.Mode = PlayerBeam.BeamMode.BOTH;
            }
            else
            {
                beam.Mode = PlayerBeam.BeamMode.REPEL;
            }

            if (playAudio != null) StopCoroutine(playAudio);
            playAudio = PlayLoopingAudio();
            StartCoroutine(playAudio);
        }

        public void OnRepelEnd()
        {
            if (grabbedObject == null)
            {
                repel = false;

                if (!attract)
                {
                   /* if (repelTime <= CLICK_TIME)
                    {
                        // Fast repel
                        FastRepelClosestObject();
                        audioSources[2].PlayOneShot(sfx.fastRepel);
                    }*/

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

            if (attract)
            {
                forceToApply *= -1;
            }

            toApply.GetComponent<Rigidbody>().AddForce(forceToApply);
        }

        private bool GrabClosestObject()
        {
            GameObject closest = FindClosestObjectInBeam();
            bool grabbed = false;
            if (closest != null && closest.activeSelf)
            {
                grabbed = GrabObject(closest);
            }

            return grabbed;
        }

        private bool GrabObject(GameObject toGrab)
        {
            bool grabbed = false;
            Grabbable grabbable = toGrab.GetComponent<Grabbable>();
            if (grabbable != null && toGrab.GetComponent<ObjectSize>().Size <= maxGrabSize)
            {
                grabbable.Grab(attachmentPoint);
                grabbedObject = grabbable;
                beam.Mode = PlayerBeam.BeamMode.OFF;
                attract = false;
                repel = false;
                grabbed = true;
            }

            return grabbed;

        }

        private void ReleaseGrabbedObject()
        {
            if (grabbedObject != null)
            {
                grabbedObject.Release();
                grabbedObject = null;
            }
        }

        private void FastRepelClosestObject()
        {
            GameObject closest = FindClosestObjectInBeam();
            if (closest != null)
            {
                Vector3 forceToApply = transform.forward * repelBurstForce;
                closest.GetComponent<Rigidbody>().AddForce(forceToApply);
            }
        }

        private GameObject FindClosestObjectInBeam()
        {
            GameObject closest = null;
            float closestDistance = 0;
            foreach (GameObject obj in beam.ObjectsInBeam)
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

        IEnumerator PlayLoopingAudio()
        {
            yield return new WaitUntil(() => !trailingOff);

            foreach (AudioSource s in audioSources)
            {
                s.volume = 1;
            }

            yield return new WaitUntil(() => attractTime > CLICK_TIME + .1f || repelTime > CLICK_TIME + .1f || beam.Mode == PlayerBeam.BeamMode.NEUTRAL || beam.Mode == PlayerBeam.BeamMode.OFF || grabbedObject != null);

            if (beam.Mode != PlayerBeam.BeamMode.NEUTRAL && grabbedObject == null && beam.Mode != PlayerBeam.BeamMode.OFF)
            {
                if (attract) audioSources[0].Play();
                if (repel) audioSources[1].Play();
            }

            yield return new WaitUntil(() => beam.Mode == PlayerBeam.BeamMode.NEUTRAL || beam.Mode == PlayerBeam.BeamMode.OFF || grabbedObject != null);

            if (audioSources[0].isPlaying || audioSources[1].isPlaying)
            {
                StartCoroutine(TrailOffAudio());
            }
        }

        IEnumerator TrailOffAudio()
        {
            trailingOff = true;
            while (audioSources[0].volume != 0)
            {
                audioSources[0].volume -= .05f;
                audioSources[1].volume -= .05f;
                yield return null;
            }
            trailingOff = false;
        }
    }
}

