using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            musicPlayers.Add("chaos", getMusicPlayers[0]);
            musicPlayers.Add("mid", getMusicPlayers[1]);
            musicPlayers.Add("order", getMusicPlayers[2]);
        }

        private void Start()
        {
            gm = GameManager.Instance;
            lastEntropy = gm.EntropyPercentage * .01f;
            musicPlayers["order"].volume = .02f + (.18f * lastEntropy);
            musicPlayers["mid"].volume = .2f - (Mathf.Abs(lastEntropy - .5f) * 2 * .15f);
            chaosVolume = .02f + (.18f * (1 - lastEntropy));
            musicPlayers["chaos"].volume = chaosVolume;
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

        IEnumerator VolumeTransition()
        {
            // order gets louder the closer it gets to 100%, midstate to 50%, chaos to 0%
            float targetOrderVol = .02f + (.18f * lastEntropy);
            float targetMidVol = .2f - (Mathf.Abs(lastEntropy - .5f) * 2 * .15f);
            float targetChaosVol = .02f + (.18f * (1 - lastEntropy));

            float transitionTimer = 1;
            while (transitionTimer > 0)
            {
                musicPlayers["chaos"].volume = Mathf.SmoothDamp(chaosVolume, targetChaosVol, ref chaosVel, 1);
                musicPlayers["order"].volume = Mathf.SmoothDamp(musicPlayers["order"].volume, targetOrderVol, ref orderVel, 1);
                musicPlayers["mid"].volume = Mathf.SmoothDamp(musicPlayers["mid"].volume, targetMidVol, ref midVel, 1);

                transitionTimer -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
