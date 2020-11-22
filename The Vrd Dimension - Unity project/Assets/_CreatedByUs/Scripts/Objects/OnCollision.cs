using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class OnCollision : MonoBehaviour
    {
        [SerializeField]
        private AudioClip wallCollision;
        IEnumerator wallSoundTimer;

        bool canChangePitch = true;

        private AudioSource audioSource;
        private Rigidbody rb;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Scenery"))
            {
                if (canChangePitch) audioSource.pitch = UnityEngine.Random.Range(.98f, 1.02f);
                if (wallSoundTimer != null) StopCoroutine(wallSoundTimer);
                wallSoundTimer = Timer(a => canChangePitch = true, wallCollision.length);
                StartCoroutine(wallSoundTimer);
                canChangePitch = false;

                float v = Mathf.Clamp(rb.velocity.magnitude / 16, .1f, 1.2f);
                audioSource.PlayOneShot(wallCollision, v);
            }
        }

        public IEnumerator Timer(Action<bool> assigner, float timer)
        {
            yield return new WaitForSeconds(timer);
            assigner(true);
        }
    }
}