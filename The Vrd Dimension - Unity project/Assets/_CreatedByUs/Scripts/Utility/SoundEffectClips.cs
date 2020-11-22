using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class SoundEffectClips : MonoBehaviour
    {
        [Header("Objects")]
        public List<AudioClip> objectWallCollision;

        [Header("Beam")]
        public AudioClip holdRepel;
        public AudioClip holdAttract;
        public AudioClip grab;
        public AudioClip fastRepel;

        [Header("Enemies")]
        public List<AudioClip> chaosSpawn;
        public List<AudioClip> chaosDeath;
        public List<AudioClip> orderSpawn;
        public List<AudioClip> orderDeath;

        public static SoundEffectClips instance;

        private void Awake()
        {
            instance = this;
        }
    }
}
