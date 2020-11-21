using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Objects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Grabbable : MonoBehaviour
    {
        private Rigidbody rb;

        private Coroutine moveRoutine;

        private static readonly float GRAB_TIME = 0.33f; // Time to move to parent

        public bool IsGrabbed
        {
            get; private set;
        }

        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void OnEnable()
        {
            IsGrabbed = false;
        }

        public void Grab(Transform parent)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            transform.parent = parent;
            IsGrabbed = true;
            moveRoutine = StartCoroutine(MoveToParent());
        }

        public void Release()
        {
            if(moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
                moveRoutine = null;
            }
            IsGrabbed = false;
            transform.parent = null;
            rb.isKinematic = false;
        }

        private IEnumerator MoveToParent()
        {
            float elapsed = 0f;
            Vector3 startPos = transform.localPosition;
            while(transform.localPosition != Vector3.zero)
            {
                yield return null;
                elapsed += Time.deltaTime;

                Vector3 newPos = Vector3.Lerp(startPos, Vector3.zero, elapsed / GRAB_TIME);
                transform.localPosition = newPos;
            }
        }
    }
}

