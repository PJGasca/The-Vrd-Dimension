using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Utility
{ 
    public class AppQuitter : MonoBehaviour
    {
        AppQuitter _Current { get; set; }


        void Awake () {
            if (_Current == null) {
                _Current = this;
                DontDestroyOnLoad (this.gameObject);
            } else if (_Current != this) {
                Destroy (this.gameObject);
            }
        }


        void Update () {
            if (Input.GetKeyUp (KeyCode.Escape)) {
                Application.Quit ();
// Commented out so that ESC only exits when not being run from editor, to allow getting control of mouse to use inspector, etc. while in mouse mode.
// (in editor, press play button or Ctrl+P to stop)
/*
#if UNITY_EDITOR
                if(EditorApplication.isPlaying) {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
#endif
*/
            }
        }
    }
}
