using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Helpers : MonoBehaviour
// any basic multi-use helper functions
{
    public static Helpers instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator Timer(Action<bool> assigner, float timer)
    {
        yield return new WaitForSeconds(timer);
        assigner(true);
    }

    public IEnumerator WaitOneFrame(Action<bool> assigner)
    {
        yield return null;
        assigner(true);
    }
}
