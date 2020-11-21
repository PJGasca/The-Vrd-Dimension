using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class BeamMouseInterface : MonoBehaviour
    {
        [SerializeField]
        private PlayerBeamController beamController;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                beamController.OnRepelStart();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                beamController.OnRepelEnd();
            }

            if (Input.GetMouseButtonDown(1))
            {
                beamController.OnAttractStart();
            }
            else if(Input.GetMouseButtonUp(1))
            {
                beamController.OnAttractEnd();
            }
        }
    }
}

