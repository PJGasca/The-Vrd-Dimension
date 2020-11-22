using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class AccelerateTowardsPoint : MonoBehaviour
    {
        public Vector3 targetPoint;

        private Rigidbody rb;

        [SerializeField]
        private float force;

        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        private void FixedUpdate()
        {
            rb.AddForce((targetPoint - transform.position).normalized * force * Time.fixedDeltaTime);
        }
    }
}

