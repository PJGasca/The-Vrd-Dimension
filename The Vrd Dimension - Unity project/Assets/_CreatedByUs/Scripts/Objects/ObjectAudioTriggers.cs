using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class ObjectAudioTriggers : MonoBehaviour
    {
        private List<AudioClip> wallCollision;
        private int sceneryLayer;

        private AudioClip chaosLoop;

        float lastSoundEndTime = 0;
        IEnumerator soundTimer;
        bool canChangePitch = true;

        private AudioSource[] audioSources;
        private Rigidbody rb;

        private void Awake()
        {
            audioSources = GetComponents<AudioSource>();
            rb = GetComponent<Rigidbody>();
            sceneryLayer = LayerMask.NameToLayer("Scenery");
        }

        private void Start()
        {
            var sfx = Utility.SoundEffectClips.instance;
            wallCollision = sfx.objectWallCollision;
            chaosLoop = sfx.chaosTracks[UnityEngine.Random.Range(0, sfx.chaosTracks.Count)];
            audioSources[1].clip = chaosLoop;
            audioSources[1].time = Time.realtimeSinceStartup % chaosLoop.length;
            audioSources[1].Play();
        }

        private void OnEnable()
        {
            if (chaosLoop != null)
            {
                audioSources[1].time = Time.realtimeSinceStartup % chaosLoop.length;
                audioSources[1].Play();
            }
        }

        private void Update()
        {
            audioSources[1].volume = Game.MusicManager.instance.chaosVolume;
        }

        private void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.layer == sceneryLayer)
            {
                if (wallCollision != null)
                {
                    int r = UnityEngine.Random.Range(0, wallCollision.Count);
                    if (canChangePitch) audioSources[0].pitch = UnityEngine.Random.Range(.98f, 1.02f);
                    CompareClipLength(wallCollision[r].length);
                    float v = Mathf.Clamp(rb.velocity.magnitude / 5 * (1 + audioSources[1].volume), .4f, 2f);
                    audioSources[0].PlayOneShot(wallCollision[r], v);
                }
            }
        }

        private void OnDisable()
        {
            audioSources[1].Pause();
        }

        void CompareClipLength(float l)
        {
            if (lastSoundEndTime < Time.realtimeSinceStartup + l)
            {
                if (soundTimer != null) StopCoroutine(soundTimer);
                soundTimer = Timer(a => canChangePitch = true, l);
                StartCoroutine(soundTimer);
                canChangePitch = false;
                lastSoundEndTime = Time.realtimeSinceStartup + l;
            }
        }

        IEnumerator Timer(Action<bool> assigner, float timer)
        {
            yield return new WaitForSeconds(timer);
            assigner(true);
        }
    }
}