using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(MoveTowardsPoint))]
    [RequireComponent(typeof(OrderAgentSeeking))]
    public class OrderAgentReturning : MonoBehaviour
    {
        private Rigidbody rb;
        private MoveTowardsPoint mover;
        public GameObject GrabbedObject;
        private OrderAgentSeeking seekingBehaviour;

        [SerializeField]
        private float objectDropOffRange;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mover = GetComponent<MoveTowardsPoint>();
            seekingBehaviour = GetComponent<OrderAgentSeeking>();
        }

        private void OnEnable()
        {
            // Put a bit of spin on it
            rb.AddRelativeTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
            mover.enabled = true;
            mover.targetPoint = GrabbedObject.GetComponent<Tetrahedron>().SpawnPosition;
        }

        public void FixedUpdate()
        {
            if(!GrabbedObject.activeSelf || !GrabbedObject.transform.parent == transform)
            {
                // Lost the object. Find a different one.
                seekingBehaviour.enabled = true;
                this.enabled = false;
            }
            else if(Vector3.Distance(transform.position, mover.targetPoint) < objectDropOffRange)
            {
                // Drop off the object
                GrabbedObject.GetComponent<Grabbable>().Release();
                GrabbedObject.transform.position = mover.targetPoint;
                seekingBehaviour.enabled = true;
                this.enabled = false;
            }
        }
    }
}
