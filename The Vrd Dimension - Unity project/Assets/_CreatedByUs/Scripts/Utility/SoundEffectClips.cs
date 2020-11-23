using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class SoundEffectClips : MonoBehaviour
    {
        [Header("Objects")]
        public List<AudioClip> objectWallCollision;
        public List<AudioClip> objectMerge;

        [Header("Beam")]
        public AudioClip holdRepel;
        public AudioClip holdAttract;
        public AudioClip grab;
        public AudioClip fastRepel;

        [Header("Enemies")]
        public List<AudioClip> chaosSpawn;
        public List<AudioClip> chaosDeath;
        public AudioClip chaosBreak;
        public List<AudioClip> orderSpawn;
        public List<AudioClip> orderDeath;
        public AudioClip orderAbsorb;

        [Header("Chaos")]
        public List<AudioClip> chaosTracks;

        [Header("Game State")]
        public AudioClip victory;

        public static SoundEffectClips instance;

        private void Awake()
        {
            instance = this;
        }
    }
}
