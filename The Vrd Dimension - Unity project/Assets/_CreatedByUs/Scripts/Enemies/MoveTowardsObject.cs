using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class MoveTowardsObject : MonoBehaviour
    {
        public GameObject TargetObject;

        private Rigidbody rb;

        [SerializeField]
        private float maxForce;

        [SerializeField]
        private float distanceToTargetVelocity;

        [SerializeField]
        private float maximumVelocity;

        [SerializeField]
        private float gain;

        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if(TargetObject!=null && TargetObject.activeSelf)
            {
                Vector3 dist = TargetObject.transform.position - transform.position;
                Vector3 targetVel = Vector3.ClampMagnitude(distanceToTargetVelocity * dist, maximumVelocity);
                // calculate the velocity error
                Vector3 error = targetVel - rb.velocity;
                // calc a force proportional to the error (clamped to maxForce)
                Vector3 force = Vector3.ClampMagnitude(gain * error, maxForce);
                rb.AddForce(force);
            }

        }
    }
}

