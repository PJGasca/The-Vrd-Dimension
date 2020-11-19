using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Paul Gasca was inspired from Daniel Perry's PerryVRSettings.cs from Musical Migration which allowed basic version of switching between two cameras.
// Paul then greatly built upon the idea and figured out how to have both VR and FPS cameras working simultaneously so that person at computer
// could move around the same world without interrupting the person's view in VR.

// TO DO: I don't think things work 100% like they should when setting the simultanous cameras/characters to "false".
// I need to confirm that Daniel's version also produces the NullReferenceException in SteamVR_Camera.cs when switching cameras
// (need to test again to see if this still happens).


public class VRAndSecondaryCameras : MonoBehaviour
{
    [SerializeField] public bool viewVRCamera = true;
    [SerializeField] public bool simultaneousCameras = true;
    [SerializeField] public bool simultaneousCharacters = true;
    
    static private AudioListener secondaryAudioListener;
    static private Camera secondaryCamera;

    [SerializeField] private GameObject hmd;
    static private AudioListener hmdAudioListener;
    [SerializeField] private GameObject secondaryCameraObject;

    private bool currentVRStatus;

    void Start()
    {
        // To disable AutoEnableVR always turning on the Vive even when Virtual Reality Supported is deactivated,
        // deactivate the Edit > Preferences > SteamVR > Automatically Enable VR checkbox.
        // I can't figure out how to do this through a script.
        //SteamVR_Preferences.AutoEnableVR = false;

        // To set whether the Vive should turn on, use the Edit > Project Settings > Player > Virtual Reality Supported checkbox.
        // If the Virtual Reality Supported checkbox is unchecked, the VR camera can't be used.
        
        secondaryCamera = secondaryCameraObject.GetComponentInChildren<Camera>();

        if (simultaneousCameras)
        {
            secondaryCamera.stereoTargetEye = StereoTargetEyeMask.None;
        }
        else
        {
            secondaryCamera.stereoTargetEye = StereoTargetEyeMask.Both;
        }

        currentVRStatus = viewVRCamera;

        //hmd = GameObject.Find("[CameraRig]");
        //Debug.Log(hmd.name);

        secondaryAudioListener = secondaryCameraObject.GetComponentInChildren<AudioListener>();
        hmdAudioListener = hmd.GetComponentInChildren<AudioListener>();

        // It seems VRCamera must be disabled and re-enabled.
        // Otherwise looking up/down is not possible with FPS view.
        hmd.SetActive(false);
        hmd.SetActive(true);

        // It seems secondaryCamera must be shown before VRCamera.
        // Otherwise looking up/down is not possible with FPS view.
        StartCoroutine(ViewSecondaryCameraFirst());


        //ViewSelectedCamera();
    }

    IEnumerator ViewSecondaryCameraFirst()
    {
        //Debug.Log("ViewSecondaryCameraFirst()");

        // It seems VR camera must not be turned off until later.
        // Otherwise VR camera does not work properly.
        bool simultaneousCamerasSelection = simultaneousCameras;
        simultaneousCameras = true;

        // Show Secondary Camera first for a single frame.
        ViewSecondaryCamera();

        // Wait one frame.
        yield return 0;

        // Now it's okay to not have both cameras on at the same time.
        simultaneousCameras = simultaneousCamerasSelection;

        ViewSelectedCamera();
    }

    private void ViewSelectedCamera()
    {
        if (viewVRCamera)
        {
            ViewVRCamera();
            secondaryCameraObject.SetActive(false);
        }
        else
        {
            ViewSecondaryCamera();
            secondaryCameraObject.SetActive(true);
        }
    }

    private void ViewSecondaryCamera()
    {
        // Even when modifying the Edit > Project Settings > Execution Order to where this script is first on list,
        // it seems that only the PlayerSettings.virtualRealitySupported setting the Unity Editor matters.
        //PlayerSettings.virtualRealitySupported = false;
        //UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Standalone, new string[] { "None", "OpenVR" });

        if(simultaneousCameras)
        {
            secondaryCamera.stereoTargetEye = StereoTargetEyeMask.None;
        }
        else
        {
            // Turn off VR camera
            UnityEngine.XR.XRSettings.showDeviceView = false;
            UnityEngine.XR.XRSettings.enabled = false;
            hmd.SetActive(false);
        }

        if (simultaneousCharacters)
        {
            // Turn on the Secondary camera only.
            secondaryCamera.enabled = true;
            secondaryAudioListener.enabled = true;
        }
        else
        {
            // Turn on the Secondary camera and Secondary chaacter.
            secondaryCameraObject.SetActive(true);
        }

        hmdAudioListener.enabled = false;

        //Debug.Log("Switched to Secondary Camera.");
        currentVRStatus = viewVRCamera;
    }

    private void ViewVRCamera()
    {
        // Even when modifying the Edit > Project Settings > Execution Order to where this script is first on list,
        // it seems that only the PlayerSettings.virtualRealitySupported setting the Unity Editor matters.
        //PlayerSettings.virtualRealitySupported = true;
        //UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Standalone, new string[] {"OpenVR", "None"});

#if UNITY_EDITOR
        if (!PlayerSettings.virtualRealitySupported)
#else
        if (false)
#endif
        {
            viewVRCamera = false;
            Debug.Log("The Edit > Project Settings > Player > Virtual Reality Supported checkbox is deactivated.");
        }
        else
        {
            if (simultaneousCameras)
            {
                secondaryCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            }

            if(simultaneousCharacters)
            {
                // Turn off the Secondary camera only.
                secondaryCamera.enabled = false;
                secondaryAudioListener.enabled = false;
            }
            else
            {
                // Turn off the Secondary camera and Secondary chaacter.
                secondaryCameraObject.SetActive(false);
            }

            // Turn on VR camera.
            UnityEngine.XR.XRSettings.showDeviceView = true;
            UnityEngine.XR.XRSettings.enabled = true;
            hmd.SetActive(true);
            hmdAudioListener.enabled = true;

            //Debug.Log("Switched to VR Camera.");
        }

        currentVRStatus = viewVRCamera;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            viewVRCamera = !viewVRCamera;
        }

        if (viewVRCamera != currentVRStatus)
        {
            //Debug.Log("Attemping to switch cameras.");

            ViewSelectedCamera();
        }
    }
}
