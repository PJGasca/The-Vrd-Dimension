using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Utility
{ 
    public class Billboard : MonoBehaviour
    {
        void Update()
        {
            /*Vector3 cameraRot = VrAndNonVrSupport.CurrentCameraObj.transform.rotation.eulerAngles;
            Vector3 cameraRotXY = new Vector3(cameraRot.x, cameraRot.y, 0);
            Quaternion cameraRotXYQuart = Quaternion.Euler(cameraRotXY);*/
            transform.LookAt(VrAndNonVrSupport.CurrentCameraObj.transform.position, Vector3.up);
        }
    }
}