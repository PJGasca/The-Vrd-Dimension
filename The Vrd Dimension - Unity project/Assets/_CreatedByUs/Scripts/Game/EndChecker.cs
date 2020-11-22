using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class EndChecker : MonoBehaviour
    {
        public static event System.Action OnGameWin;


        void OnEnable () {
            Objects.MergableObjectManager.OnMergeUpdate += CheckForWin;
        }


        void OnDisable () {
            Objects.MergableObjectManager.OnMergeUpdate -= CheckForWin;
        }


        void CheckForWin (int tetrahedronCount) {
            if (tetrahedronCount <= 1)
                Win ();
        }


        void Win () {
            if (OnGameWin != null) { OnGameWin (); }
        }
    }
}
