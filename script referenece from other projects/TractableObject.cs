using UnityEngine;
using System.Collections;
using System;

public class TractableObject : MonoBehaviour
{
    [SerializeField]
    private CreatureController objectGravity;
    [SerializeField]
    private Rigidbody rBody;
    private bool isCreatureTracted = false;
    private TractionBeamController currBeamController;

    private float relativeDistance;
    private float initialRelativeDistance;
    private Vector3 relativeDirection;
    private Quaternion relativeRotation;
    private Quaternion controllerInitialRotation;

    //private Vector3[] lastPositions = new Vector3[3];
    //private float[] lastPositionTimes = new float[3];
    private float maxDistance = 5;
    private float minDistance = 0.1f;
    private ParticleSystem beamPFX;
    private float beamLengthFixer;

    // Stores the values set in the editor as the defaults.
    private GameObject defaultParent;
    private Vector3 defaultScale;

    private bool swallowCreature;

    void Awake()
    {
        // Store the values set in the editor as the defaults.
        defaultParent = transform.parent.gameObject;
        defaultScale = transform.localScale;

        swallowCreature = true;
    }

    void FixedUpdate()
    {
        if (isCreatureTracted)
        {
            TractedUpdate();
        }
    }

    private void TractedUpdate()
    {
        Vector3 newPosition = currBeamController.transform.position + (currBeamController.transform.forward * relativeDistance) + (relativeDirection * initialRelativeDistance);
        rBody.velocity = (newPosition - transform.position) * currBeamController.tractionVelocityStrength;

        if (relativeDistance > 0.05f) { relativeDistance -= 0.02f; }
        Quaternion newRotation = currBeamController.controller_Event.transform.rotation * Quaternion.Inverse(controllerInitialRotation);
        newRotation = newRotation * relativeRotation;
        transform.rotation = newRotation;
        //float angle;
        //Vector3 axis;
        //newRotation.ToAngleAxis(out angle, out axis);

        //if (angle > 180)
        //    angle -= 360;

        //rBody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * currBeamController.rotationVelocityStrength;

        //UpdatePositionHistory(newPosition); //used to measure last 3 frames - Currently disabled
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Suction" && isCreatureTracted && swallowCreature)
        {
            currBeamController.SwallowCreature(transform);
            //swallowCreature = false;
        }
    }

    public void InitiateCreatureGrabbed(TractionBeamController beamController)
    {
        objectGravity.SetGravity(false);
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;


        // to prevent nulls in position or using old information to calculate release force, initializing all frames - currently disabled
        //for (int i = 0; i < lastPositions.Length; i++)
        //{
        //    lastPositions[i] = transform.position;
        //    lastPositionTimes[i] = 0;
        //}


        isCreatureTracted = true;
        currBeamController = beamController;
        relativeDistance = Vector3.Distance(transform.position, currBeamController.transform.position);
        initialRelativeDistance = relativeDistance;
        relativeDirection = transform.position - currBeamController.transform.position;
        relativeDirection = relativeDirection.normalized - currBeamController.transform.forward;
        relativeRotation = Quaternion.identity * transform.rotation;
        controllerInitialRotation = beamController.controller_Event.transform.rotation;
        beamPFX = currBeamController.BeamPFX;
        //beamLengthFixer = currBeamController.beamLengthFixer;
        //beamPFX.startLifetime = relativeDistance * beamPFX.startSpeed * beamLengthFixer;
    }

    public void ResetCreatureGrabbed()
    {
        //gameObject.SetActive(false);
        //gameObject.SetActive(true);
        objectGravity.SetGravity(true);
        isCreatureTracted = false;
        currBeamController = null;

        // Detach creature from the UFO object and set its parent back to default.
        transform.parent = defaultParent.transform;

        // Enlarge the creature back to its original size.
        transform.localScale = defaultScale;

        // Turn the object's physics back on (seems to pernamently break physics).
        //GetComponent<Rigidbody>().isKinematic = true;
    }

    //private void UpdatePositionHistory(Vector3 newPosition)
    //{
    //    lastPositions[2] = lastPositions[1];
    //    lastPositions[1] = lastPositions[0];
    //    lastPositions[0] = newPosition;
    //    lastPositionTimes[2] = lastPositionTimes[1];
    //    lastPositionTimes[1] = lastPositionTimes[0];
    //    lastPositionTimes[0] = Time.deltaTime;
    //}

    public void UpdateCreatureDistance(float change)
    {
        if ((change > 0 && relativeDistance < maxDistance) || (change < 0 && relativeDistance > minDistance))
        {
            relativeDistance += change * Time.deltaTime;
            //beamPFX.startLifetime = relativeDistance * beamPFX.startSpeed * beamLengthFixer;
        }
    }
}
