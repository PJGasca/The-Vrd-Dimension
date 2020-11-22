using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class MusicManager : MonoBehaviour
    {
        IEnumerator volumeTransition;
        float timer;
        Dictionary<string, AudioSource> musicPlayers = new Dictionary<string, AudioSource>();
        float chaosVel, orderVel, midVel;

        public static MusicManager instance;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            AudioSource[] getMusicPlayers = GetComponents<AudioSource>();
            musicPlayers.Add("chaos", getMusicPlayers[0]);
            musicPlayers.Add("mid", getMusicPlayers[1]);
            musicPlayers.Add("order", getMusicPlayers[2]);
        }

        private void Start()
        {
            float currentEntropy = GameManager.Instance.EntropyPercentage;
            musicPlayers["order"].volume = .05f + (.95f * currentEntropy);
            musicPlayers["mid"].volume = 1 - (Mathf.Abs(currentEntropy - .5f) * 2 * .9f);
            musicPlayers["chaos"].volume = .05f + (.95f * (1 - currentEntropy));
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
                float targetOrderVolume = .05f + (.95f * GameManager.Instance.EntropyPercentage);
                if (musicPlayers["order"].volume - .001f < targetOrderVolume || musicPlayers["order"].volume + .001f > targetOrderVolume)
                {
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

        IEnumerator VolumeTransition()
        {
            // order gets louder the closer it gets to 100%, midstate to 50%, chaos to 0%
            float currentEntropy = GameManager.Instance.EntropyPercentage;

            float targetOrderVol = .05f + (.95f * currentEntropy);
            float targetMidVol = 1 - (Mathf.Abs(currentEntropy - .5f) * 2 * .9f);
            float targetChaosVol = .05f + (.95f * (1 - currentEntropy));

            targetOrderVol = musicPlayers["order"].volume - targetOrderVol;
            targetMidVol = musicPlayers["mid"].volume - targetMidVol;
            targetChaosVol = musicPlayers["chaos"].volume - targetChaosVol;

            float transitionTimer = 1;
            while (transitionTimer > 0)
            {
                musicPlayers["chaos"].volume = Mathf.SmoothDamp(musicPlayers["chaos"].volume, targetChaosVol, ref chaosVel, 1);
                musicPlayers["order"].volume = Mathf.SmoothDamp(musicPlayers["order"].volume, targetOrderVol, ref orderVel, 1);
                musicPlayers["mid"].volume = Mathf.SmoothDamp(musicPlayers["mid"].volume, targetMidVol, ref midVel, 1);

                transitionTimer -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
