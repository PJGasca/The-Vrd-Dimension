using UnityEngine;
using System.Collections;
using VRTK;
using System;

public class TractionBeamController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField]
    public VRTK_ControllerEvents controller_Event;
    [SerializeField]
    public float tractionVelocityStrength = 20;
    [SerializeField]
    public float rotationVelocityStrength = 20;
    [SerializeField]
    private Transform shootLocation;

    // Set this to the UFO's dome object as reference of where to place swallowed creatures.
    [SerializeField]
    private GameObject dome;

    private bool isCreatureGrabbed = false;

    [Header("PFX Settings")]
    [SerializeField]
    private ParticleSystem beamPFX;
    public ParticleSystem BeamPFX
    {
        get { return beamPFX; }
        set { beamPFX = value; }
    }
    [Header("Raycast Settings")]
    [SerializeField]
    private float rayDistance = 20;
    [SerializeField]
    public float beamLengthFixer = 0.45f;
    private RaycastHit rayHitInfo;
    private int layerIndexIgnore = ~(1 << 8);
    private TractableObject currentTractableObject;

    public bool triggerButtonDown = false;
    public bool touchpadButtonDown = false;
    private bool isCreatureInsideController = false;
    private bool waitForRecharge = false;


    [Header("Physics Settings")]
    [SerializeField]
    private float shootSpeed = 1;
    private GameObject currentlySwallowedCreature;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource audioSource;
    [Space]
    [SerializeField]
    private AudioClip tractionStartedClip;
    [SerializeField]
    private AudioClip[] shooterClips;
    [SerializeField]
    private AudioClip rechargeClip;
    [SerializeField]
    private AudioClip swallowCreatureClip;

    void Start()
    {

        controller_Event.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        controller_Event.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);
        controller_Event.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        controller_Event.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);
        audioSource.Stop();
        audioSource.loop = false;
    }

    
    void Update()
    {

        if (controller_Event == null)
        {

            Debug.Log("Controller Events not assigned");

            return;

        }

        if (triggerButtonDown)
        {
            DoTriggerPressedUpdate();
        }

        //if (touchpadButtonDown & isCreatureGrabbed)
        //{
        //    UpdateCreatureLocation();
        //}
    }

    private void DoTriggerPressedUpdate()
    {
        //Vector3 fwd = transform.TransformDirection(Vector3.forward);
        

        //Debug.DrawRay(transform.position, transform.forward * rayDistance);
        if (!isCreatureGrabbed && !waitForRecharge)
        {
            Ray hittingRay = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(hittingRay, out rayHitInfo, rayDistance, layerIndexIgnore))
            {
                //Debug.Log(rayHitInfo.collider.tag);
                if (rayHitInfo.collider.isTrigger && rayHitInfo.collider.tag == "Creature")
                {
                    //Debug.Log("raycast isgrabbing " + rayHitInfo.transform.name);
                    currentTractableObject = rayHitInfo.transform.GetComponent<TractableObject>();
                    InitiateCreatureGrabbed();
                }
            }
        }
    }

    private void UpdateCreatureLocation()
    {
        Vector2 touchpadAxis = controller_Event.GetTouchpadAxis();
        if (touchpadAxis.x > touchpadAxis.y && touchpadAxis.x < -touchpadAxis.y)
        {
            currentTractableObject.UpdateCreatureDistance(-1);
        }
        else if (touchpadAxis.x < touchpadAxis.y && touchpadAxis.x > -touchpadAxis.y)
        {
            currentTractableObject.UpdateCreatureDistance(1);
        }
    }

    private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        triggerButtonDown = false;

        beamPFX.Stop();
        ResetPFX();

        // Let go of creature when releasing trigger and creature is outside UFO.
        if (isCreatureGrabbed && !isCreatureInsideController)
        {
            ResetCreatureGrabbed();

            // For some reason, this seems to be needed both before and after resetting the creature, or there are some beam glitches.
            ResetPFX();
        }
    }


    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (isCreatureInsideController)
        {
            StartCoroutine(ShootCreature());
            ResetCreatureGrabbed();
        }
        else
        {
            triggerButtonDown = true;
            beamPFX.Play();
            audioSource.clip = tractionStartedClip;
            audioSource.Play();
        }
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        touchpadButtonDown = false;
    }

    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        touchpadButtonDown = true;
    }

    private void ResetCreatureGrabbed()
    {
        currentTractableObject.ResetCreatureGrabbed();
        currentTractableObject = null;
        currentlySwallowedCreature = null;
        isCreatureGrabbed = false;
        isCreatureInsideController = false;
    }

    private void InitiateCreatureGrabbed()
    {
        currentTractableObject.InitiateCreatureGrabbed(this);
        isCreatureGrabbed = true;
        StartPFXFollow();
    }

    public void SwallowCreature(Transform thisTransform)
    {
        // This function is currently called every cycle even though ideally it should only be called once. Doesn't seem to work correctly though when only called once.
        //Debug.Log("SwallowCreature()");

        currentlySwallowedCreature = thisTransform.gameObject;
        //thisTransform.SetParent(this.transform);

        // Creature seems to move slower when put back on planet if it isn't deactivated.
        currentlySwallowedCreature.SetActive(false);
        currentlySwallowedCreature.SetActive(true);

        isCreatureInsideController = true;
        ResetPFX();

        // Plays repeatedly since this function currently is called every cycle while creature is inside ship.
        //audioSource.clip = swallowCreatureClip;
        //audioSource.Play();

        // Attach creature to center of UFO dome object.
        currentlySwallowedCreature.transform.parent = dome.transform;
        currentlySwallowedCreature.transform.position = dome.transform.position;

        // Shrink the creature to fit inside the dome.
        currentlySwallowedCreature.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);


        // My attempts to lock orientation of creature to UFO:

        // Turn off the physics for creature (doesn't work and seems to pernamently break physics).
        //currentlySwallowedCreature.GetComponent<Rigidbody>().isKinematic = false;

        /*
        // Try to keep rigidBody from moving without disabling or destroying it (doesn't work).
        currentlySwallowedCreature.GetComponent<Rigidbody>().Sleep();
        currentlySwallowedCreature.GetComponent<Rigidbody>().velocity = Vector3.zero;
        currentlySwallowedCreature.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        */
    }

    private void StartPFXFollow()
    {
        beamPFX.transform.SetParent(currentTractableObject.transform);
        beamPFX.transform.localPosition = Vector3.zero;
        beamPFX.GetComponent<TractorPFXController>().StartFollowing();
    }

    private void ResetPFX()
    {
        beamPFX.Stop();
        beamPFX.GetComponent<TractorPFXController>().StopFollowing();
        beamPFX.transform.localPosition = Vector3.zero;
        beamPFX.transform.SetParent(transform);
        beamPFX.transform.rotation = transform.rotation;
    }

    private IEnumerator ShootCreature()
    {
        waitForRecharge = true;
        currentlySwallowedCreature.transform.position = shootLocation.position;
        currentlySwallowedCreature.GetComponent<Rigidbody>().velocity = transform.forward * shootSpeed;
        //currentlySwallowedCreature.GetComponent<CreatureController>().CheckIfFloatingAimlesslyMethod();
        audioSource.clip = shooterClips[UnityEngine.Random.Range(0, shooterClips.Length)];
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.clip = rechargeClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        waitForRecharge = false;

    }
}