using UnityEngine;
using System.Collections;

public class TractorPFXController : MonoBehaviour {

    private bool isFollowingController;
    private Transform tractionControllerTransform;

    void Start ()
    {
        tractionControllerTransform = transform.parent.transform;
    }

    void Update ()
    {
        if (isFollowingController)
        {
            Vector3 followDirection = tractionControllerTransform.position - transform.position;
            transform.forward = followDirection;
        }
    }

    public void StartFollowing()
    {
        isFollowingController = true;
    }
    public void StopFollowing()
    {
        isFollowingController = false;
    }
}
