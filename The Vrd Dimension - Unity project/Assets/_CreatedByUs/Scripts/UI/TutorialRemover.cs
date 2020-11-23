using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TutorialRemover : MonoBehaviour
    {
        [SerializeField]
        private float timeToDisable;

        public void OnEnable()
        {
            StartCoroutine(TimedDisable());
        }

        private IEnumerator TimedDisable()
        {
            yield return new WaitForSeconds(timeToDisable);
            gameObject.SetActive(false);
        }
    }
}

