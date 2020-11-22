using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(MoveTowardsPoint))]
    [RequireComponent(typeof(OrderAgentReturning))]
    [RequireComponent(typeof(OrderAgentDying))]
    [RequireComponent(typeof(Collider))]
    public class OrderAgentSeeking : MonoBehaviour
    {
        private Rigidbody rb;
        private MoveTowardsPoint mover;

        private Vector3 defaultScale;

        private GameObject target = null;

        [SerializeField]
        private float targetObjectGrabRange;

        [SerializeField]
        [Tooltip("Distance from start point that an item has to be before it is considered displaced.")]
        private float displacementRange;

        private Grabbable targetGrabbable;

        private OrderAgentReturning returningBehaviour;

        private OrderAgentDying dyingBehaviour;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mover = GetComponent<MoveTowardsPoint>();
            dyingBehaviour = GetComponent<OrderAgentDying>();
            returningBehaviour = GetComponent<OrderAgentReturning>();
        }

        private void OnEnable()
        {
            // Put a bit of spin on it
            rb.AddRelativeTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
            targetGrabbable = null;
            target = null;
            GetComponent<Collider>().enabled = true;
        }

        public void FixedUpdate()
        {
            if(target == null || !target.activeSelf || targetGrabbable.IsGrabbed)
            {
                if(target!=null)
                {
                    target.GetComponent<Tetrahedron>().targetedByAgent = false;
                }
                PickNewTarget();
            }
            else if(Vector3.Distance(transform.position, target.transform.position) < targetObjectGrabRange)
            {
                Debug.Log("Is in grab range");
                Grabbable grabbable = target.GetComponent<Grabbable>();
                if (!grabbable.IsGrabbed)
                {
                    Debug.Log("Grabbing");
                    target.GetComponent<Grabbable>().Grab(transform);
                    returningBehaviour.GrabbedObject = target;
                    returningBehaviour.enabled = true;
                    this.enabled = false;
                }
                else
                {
                    PickNewTarget();
                }
            }
            else
            {
                mover.targetPoint = target.transform.position;
            }
        }

        private void PickNewTarget()
        {
            Tetrahedron[] tetras = Tetrahedron.All;

            if(target!=null)
            {
                target.GetComponent<Tetrahedron>().targetedByAgent = false;
            }

            target = null;
            foreach(Tetrahedron tetra in tetras)
            {
                if(IsDisplaced(tetra) && !tetra.targetedByAgent && !tetra.GetComponent<Grabbable>().IsGrabbed)
                {
                    target = tetra.gameObject;
                    mover.targetPoint = target.transform.position;
                    mover.enabled = true;
                    targetGrabbable = target.GetComponent<Grabbable>();
                    tetra.targetedByAgent = true;
                    break;
                }
            }

            // Nothing to pick? My work here is done!
            if (target == null)
            {
                Debug.Log("Could not find valid target. Dying.");
                mover.enabled = false;
                Die();
            }
        }

        private bool IsDisplaced(Tetrahedron tetra)
        {
            return Vector3.Distance(tetra.transform.position, tetra.SpawnPosition) > displacementRange;
        }

        private void Die()
        {
            this.enabled = false;
            dyingBehaviour.enabled = true;
        }
    }
}
