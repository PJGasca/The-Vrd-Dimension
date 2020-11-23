using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Game
{
    public class MusicManager : MonoBehaviour
    {
        IEnumerator volumeTransition;
        float timer = 0;
        Dictionary<string, AudioSource> musicPlayers = new Dictionary<string, AudioSource>();
        float chaosVel, orderVel, midVel;
        float lastEntropy;

        public float chaosVolume { get; private set; }

        GameManager gm;

        public static MusicManager instance;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            AudioSource[] getMusicPlayers = GetComponents<AudioSource>();
            musicPlayers.Add("mid", getMusicPlayers[0]);
            musicPlayers.Add("order", getMusicPlayers[1]);
        }

        private void Start()
        {
            gm = GameManager.Instance;
            lastEntropy = gm.EntropyPercentage * .01f;
            musicPlayers["order"].volume = .01f + (.14f * (1 - lastEntropy));
            musicPlayers["mid"].volume = .15f - (Mathf.Abs(lastEntropy - .5f) * 2 * .13f);
            chaosVolume = .1f + (.6f * lastEntropy);
        }

        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 1.2f;
                if (gm.EntropyPercentage * .01f != lastEntropy)
                {
                    lastEntropy = gm.EntropyPercentage * .01f;
                    //Debug.Log(GameManager.Instance.EntropyPercentage);
                    UpdateMusicVolume();
                }
            }
        }

        public void UpdateMusicVolume()
        {
            if (volumeTransition != null) StopCoroutine(volumeTransition);
            volumeTransition = VolumeTransition();
            StartCoroutine(volumeTransition);
        }

        public void PlayWinClip()
        {
            musicPlayers["order"].PlayOneShot(Utility.SoundEffectClips.instance.victory);
        }

        IEnumerator VolumeTransition()
        {
            // order gets louder the closer it gets to 0%, midstate to 50%, chaos to 100%
            float targetOrderVol = .01f + (.14f * (1 - lastEntropy));
            float targetMidVol = .15f - (Mathf.Abs(lastEntropy - .5f) * 2 * .13f);
            float targetchaosVol = .1f + (.6f * lastEntropy);

            float transitionTimer = 1;
            while (transitionTimer > 0)
            {
                chaosVolume = Mathf.SmoothDamp(chaosVolume, targetchaosVol, ref chaosVel, 1);
                musicPlayers["order"].volume = Mathf.SmoothDamp(musicPlayers["order"].volume, targetOrderVol, ref orderVel, 1);
                musicPlayers["mid"].volume = Mathf.SmoothDamp(musicPlayers["mid"].volume, targetMidVol, ref midVel, 1);

                transitionTimer -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
