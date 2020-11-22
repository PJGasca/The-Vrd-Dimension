using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Objects
{
    [ExecuteInEditMode]
    public class ObjectColor : MonoBehaviour
    {
        [SerializeField]
        private Color manualColor;
        public Color randomColor;
        private Material tetraMaterial;

        public static bool functionCall;

        // Start is called before the first frame update
        void Start()
        {
            if (manualColor.a != 0)
            {
                randomColor = manualColor;
            }
            else
            {
                float r = UnityEngine.Random.Range(0, 255);
                float g = UnityEngine.Random.Range(0, 255);
                float b = UnityEngine.Random.Range(0, 255);
                randomColor = new Color(r, g, b, 1);
            }

            tetraMaterial = GetComponent<MeshRenderer>().material;
            tetraMaterial.SetColor("_BaseColor", randomColor);
            tetraMaterial.SetColor("_EmissiveColor", randomColor);
            if (!functionCall)
            {
                functionCall = true;
                StartCoroutine(WaitOneFrame(r => Resources.UnloadUnusedAssets()));
            }
        }

        public IEnumerator WaitOneFrame(Action<bool> assigner)
        {
            yield return null;
            assigner(true);
        }
    }
}
