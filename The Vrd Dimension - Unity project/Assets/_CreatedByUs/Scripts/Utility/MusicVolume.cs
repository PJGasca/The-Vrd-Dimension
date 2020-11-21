using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolume : MonoBehaviour
{
    IEnumerator volumeTransition;
    float currentVolume;

    public static MusicVolume instance; 

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void ChangeMusicVolume()
    {
        if (volumeTransition != null) StopCoroutine(volumeTransition);
        volumeTransition = VolumeTransition();
        StartCoroutine(volumeTransition);
    }

    public float GetMusicVolume()
    {
        return currentVolume;
    }

    IEnumerator VolumeTransition()
    {
        // access global order/chaos percentage
        // (need to figure out what the conversion rate between order/chaos and music volume is first)
        // (and then transition over time between that and the current volume)
        yield return null;
    }
}
