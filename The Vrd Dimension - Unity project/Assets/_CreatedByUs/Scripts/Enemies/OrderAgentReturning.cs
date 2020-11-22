using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(MoveTowardsPoint))]
    [RequireComponent(typeof(OrderAgentSeeking))]
    [RequireComponent(typeof(OrderAgentDying))]
    public class OrderAgentReturning : MonoBehaviour
    {
        private Rigidbody rb;
        private MoveTowardsPoint mover;
        public GameObject GrabbedObject;
        private OrderAgentSeeking seekingBehaviour;
        private OrderAgentDying dyingBehaviour;

        [SerializeField]
        private float objectDropOffRange;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mover = GetComponent<MoveTowardsPoint>();
            seekingBehaviour = GetComponent<OrderAgentSeeking>();
            dyingBehaviour = GetComponent<OrderAgentDying>();
        }

        private void OnEnable()
        {
            // Put a bit of spin on it
            rb.AddRelativeTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
            mover.enabled = true;
            mover.targetPoint = GrabbedObject.GetComponent<MergableObject>().SpawnPosition;
        }

        public void OnDisable()
        {
            // If we still have an object, make sure we drop it.
            if(GrabbedObject != null)
            {
                GrabbedObject.GetComponent<Grabbable>().Release();
            }
        }

        public void FixedUpdate()
        {
            if(!GrabbedObject.activeSelf || !GrabbedObject.transform.parent == transform)
            {
                // Lost the object. Find a different one.
                GrabbedObject = null;
                seekingBehaviour.enabled = true;
                this.enabled = false;
            }
            else if(Vector3.Distance(transform.position, mover.targetPoint) < objectDropOffRange)
            {
                // Drop off the object
                GrabbedObject.GetComponent<Grabbable>().Release();
                GrabbedObject.transform.position = mover.targetPoint;
                GrabbedObject = null;
                dyingBehaviour.enabled = true;
                this.enabled = false;
            }
        }
    }
}
