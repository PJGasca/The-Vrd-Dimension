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
            List<int> colorRGB = new List<int>();
            if (manualColor.a != 0)
            {
                randomColor = manualColor;
            }
            else
            {
                List<int> randomizer = new List<int> { 0, 1, 2 };
                for (int i = 0; i < 3; i++)
                {
                    int random = UnityEngine.Random.Range(0, randomizer.Count);
                    colorRGB.Add(randomizer[random]);
                    randomizer.RemoveAt(random);
                }
                for (int i = 0; i < 3; i++)
                {
                    if (colorRGB[i] == 2)
                    {
                        colorRGB[i] = UnityEngine.Random.Range(0, 40);
                    }
                }
            }

            randomColor = new Color(colorRGB[0], colorRGB[1], colorRGB[2], 1);
            tetraMaterial = GetComponent<MeshRenderer>().material;
            tetraMaterial.SetColor("_BaseColor", randomColor);
            tetraMaterial.SetColor("_EmissiveColor", randomColor);
            tetraMaterial.SetColor("_Color", randomColor);
            if (!functionCall)
            {
                functionCall = true;
                StartCoroutine(WaitOneFrame(r => Resources.UnloadUnusedAssets()));
            }

        }
        IEnumerator WaitOneFrame(Action<bool> assigner)
        {
            yield return null;
            assigner(true);
        }
    }
}