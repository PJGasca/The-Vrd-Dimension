using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class PlayAreaManager : MonoBehaviour
    {
        void OnEnable () {
            EndChecker.OnGameWin += DeactivateWalls;
        }


        void OnDisable () {
            EndChecker.OnGameWin -= DeactivateWalls;
        }


        void DeactivateWalls () {
            gameObject.SetActive (false);
        }
    }
}
