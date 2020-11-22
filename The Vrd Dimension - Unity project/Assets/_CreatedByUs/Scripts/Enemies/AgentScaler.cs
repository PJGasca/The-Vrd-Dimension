﻿using Assets.Scripts.Objects;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class AgentScaler : MonoBehaviour
    {
        private Vector3 initialScale;

        private Coroutine scaleRoutine;

        [SerializeField]
        private float tetraScaleFactor;

        private void OnEnable()
        {
            initialScale = transform.localScale;
        }

        public void ResetScale()
        {
            this.transform.localScale = initialScale;
        }

        public void ScaleToInitial(float time)
        {
            SetScale(initialScale, time);
        }

        public void ScaleToTetra(GameObject tetra, float time)
        {
            SetScale(tetra.transform.localScale * tetraScaleFactor, time);
        }

        public void SetScale(Vector3 newScale, float time)
        {
            if(scaleRoutine!=null)
            {
                StopCoroutine(scaleRoutine);
                scaleRoutine = null;
            }

            scaleRoutine = StartCoroutine(ScaleRoutine(newScale, time));
        }

        private IEnumerator ScaleRoutine(Vector3 destScale, float time)
        {
            float elapsed = 0f;
            Vector3 initialScale = transform.localScale;
            while(transform.localScale != destScale)
            {
                yield return null;
                elapsed += Time.deltaTime;
                float t = elapsed / time;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                if(t>1)
                {
                    t = 1;
                }
                Vector3 newScale = Vector3.Lerp(initialScale, destScale, t);
                transform.localScale = newScale;
            }
            scaleRoutine = null;
        }
    }
}
