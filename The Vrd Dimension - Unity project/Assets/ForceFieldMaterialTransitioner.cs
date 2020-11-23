using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ForceFieldMaterialTransitioner : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainWall;
        Material mainWallMaterial;

        Material forceField;
        IEnumerator changeMaterial;
        float timer;
        float fresnalVel, alphaVel;

        GameManager gm;
        float lastEntropy;

        void Awake()
        {
            forceField = GetComponent<MeshRenderer>().sharedMaterial;
            mainWallMaterial = mainWall.GetComponent<MeshRenderer>().sharedMaterial;
            forceField.SetFloat("Vector1_EF88822F", 3);
            Color bc = Color.white;
            bc.a = 1;
            mainWallMaterial.SetColor("_BaseColor", bc);
        }

        // Start is called before the first frame update
        void Start()
        {
            gm = GameManager.Instance;
            lastEntropy = gm.EntropyPercentage * .01f;
            forceField.SetFloat("Vector1_EF88822F", 3 - ((1 - lastEntropy) * 3));
            Color newColor = mainWallMaterial.GetColor("_BaseColor");
            newColor.a = 1 - lastEntropy;
            mainWallMaterial.SetColor("_BaseColor", newColor);
        }

        // Update is called once per frame
        void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 1.2f;
                if (lastEntropy != gm.EntropyPercentage * .01f)
                {
                    lastEntropy = gm.EntropyPercentage * .01f;
                    if (changeMaterial != null) StopCoroutine(changeMaterial);
                    changeMaterial = ChangeForceFieldMaterial();
                    StartCoroutine(changeMaterial);
                }
            }
        }

        IEnumerator ChangeForceFieldMaterial()
        {
            float newFresnal = 3 - ((1 - lastEntropy) * 3);
            float newAlpha = 1 - lastEntropy;
            float currentAlpha = mainWallMaterial.GetColor("_BaseColor").a;

            float timer = 1;
            while (timer > 0)
            {
                forceField.SetFloat("Vector1_EF88822F", Mathf.SmoothDamp(forceField.GetFloat("Vector1_EF88822F"), newFresnal, ref fresnalVel, 1));
                currentAlpha = Mathf.SmoothDamp(currentAlpha, newAlpha, ref alphaVel, 1);
                Color newColor = mainWallMaterial.GetColor("_BaseColor");
                newColor.a = currentAlpha;
                mainWallMaterial.SetColor("_BaseColor", newColor);
                timer -= Time.deltaTime;
                yield return null;
            }
        }
    }
}