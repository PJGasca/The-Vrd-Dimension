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

        float lastSoundEndTime = 0;
        IEnumerator soundTimer;
        bool canChangePitch = true;

        private AudioSource audioSource;
        private Rigidbody rb;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();
            sceneryLayer = LayerMask.NameToLayer("Scenery");
        }

        private void Start()
        {
            var sfx = Utility.SoundEffectClips.instance;
            if (sfx != null)
            {
                if (sfx.objectWallCollision != null) wallCollision = sfx.objectWallCollision;
            }
        }

        private void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.layer == sceneryLayer)
            {
                if (wallCollision != null)
                {
                    int r = UnityEngine.Random.Range(0, wallCollision.Count);
                    if (canChangePitch) audioSource.pitch = UnityEngine.Random.Range(.98f, 1.02f);
                    CompareClipLength(wallCollision[r].length);
                    float v = Mathf.Clamp(rb.velocity.magnitude / 16, .1f, 1.2f);
                    audioSource.PlayOneShot(wallCollision[r], v);
                }
            }
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