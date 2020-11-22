using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class WinNotification : MonoBehaviour
    {
        public MeshRenderer meshRenderer;


        void OnEnable () {
            meshRenderer.enabled = false;
            EndChecker.OnGameWin += Show;
        }


        void OnDisable () {
            EndChecker.OnGameWin -= Show;
        }


        void Show () {
            meshRenderer.enabled = true;
        }
    }
}
