using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class OnCollision : MonoBehaviour
    {
        [SerializeField]
        private AudioClip wallCollision;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Scenery"))
            {
                audioSource.PlayOneShot(wallCollision);
            }
        }
    }
}