using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Valve.VR;

// The intention of this script is to be conntain all generic logic for handling VR and non-VR options for a game so that it can be easily reused.

// Paul recommends adding this script to the top of the Script Execution Order in project settings.
// It reduces number of errors/warnings when VR device is installed but not active, but doesn't eliminate all warnings/errors.
// Perhaps this could be used as reference for non-crunch time: https://answers.unity.com/questions/1670713/initialization-before-awake-possible.html

// Future plans from Paul: get simultaneous player support working, like was working in my Retrospective project: ReferenceFromOtherProjects/VRAndSecondaryCameras.cs

namespace Assets.Scripts.Utility
{
    public class VrAndNonVrSupport : MonoBehaviour
    {
        [SerializeField]
        private GameObject vrCameraRig;

        [SerializeField]
        private GameObject nonVRCameraRig;

        public GameObject currentCameraObj { get; private set; }

        private static bool vrDetected = false;

        public static event System.Action OnVRToggle;

        private bool vr = true;

        public bool VR
        {
            get { return vr; }

            set
            {
                
                if (value)
                {
                    if (SteamVR_Behaviour.instance != null)
                    {
                        vr = value;
                        SteamVR_Behaviour.instance.gameObject.SetActive(true);
                        nonVRCameraRig.SetActive(false);
                        vrCameraRig.SetActive(true);
                        currentCameraObj = vrCameraRig;
                    }
                    else
                    {
                        Debug.Log("Cannot enable VR.  [SteamVR] object not found.");
                    }
                }
                else
                {
                    vr = value;
                    vrCameraRig.SetActive(false);
                    nonVRCameraRig.SetActive(true);
                    if (SteamVR_Behaviour.instance != null)
                    {
                        SteamVR_Behaviour.instance.gameObject.SetActive(false);
                    }
                    currentCameraObj = nonVRCameraRig;
                }
            }
        }

        public void Awake()
        {
            if (!XRDevice.isPresent)
            {
                Debug.Log("No VR device found");
                XRSettings.LoadDeviceByName("None");
                SetVREnabled(false);
                VR = false;
            }
            else
            {
                vrDetected = true;
                //SetVREnabled(Preferences.GetVRPreferred());
                VR = true;
            }
        }

        public void SetVREnabled(bool enabled)
        {
            XRSettings.enabled = enabled;
            //Player.Instance.VR = enabled;
            this.VR = enabled;

            if (OnVRToggle != null)
            {
                OnVRToggle();
            }
        }

        public void Update()
        {
            if (vrDetected)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    SetVREnabled(!VR);
                }
                else if (VR && Input.GetMouseButtonDown(0))
                {
                    SetVREnabled(false);
                }
            }
        }
    }
}