using Assets.Scripts.Game;
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
    [RequireComponent(typeof(AgentScaler))]
    public class OrderAgentSeeking : MonoBehaviour
    {
        private Rigidbody rb;
        private MoveTowardsPoint mover;

        private Vector3 defaultScale;

        private GameObject target = null;

        [SerializeField]
        private float targetObjectGrabRange;

        [SerializeField]
        private float agentRescaleRange;

        [SerializeField]
        private float agentRescaleTime;

        private Grabbable targetGrabbable;

        private OrderAgentReturning returningBehaviour;

        private OrderAgentDying dyingBehaviour;

        private AgentScaler scaler;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mover = GetComponent<MoveTowardsPoint>();
            dyingBehaviour = GetComponent<OrderAgentDying>();
            returningBehaviour = GetComponent<OrderAgentReturning>();
            scaler = GetComponent<AgentScaler>();
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
                PickNewTarget();
            }
            else if (Vector3.Distance(transform.position, target.transform.position) < agentRescaleRange && transform.localScale != target.transform.localScale)
            {
                scaler.SetScale(target.transform.localScale, agentRescaleTime);
            }
            else if(Vector3.Distance(transform.position, target.transform.position) < targetObjectGrabRange && transform.localScale == target.transform.localScale)
            {
                Grabbable grabbable = target.GetComponent<Grabbable>();
                if (!grabbable.IsGrabbed)
                {
                    scaler.ScaleToTetra(target, 0.33f);
                    target.GetComponent<Grabbable>().Grab(transform);
                    target.GetComponent<Tetrahedron>().targetedByAgent = false;
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

            scaler.ScaleToInitial(0.33f);

            if (target!=null)
            {
                target.GetComponent<Tetrahedron>().targetedByAgent = false;
            }

            Tetrahedron tetra = GameManager.Instance.GetDisplacedUntargetedTetra();
            if (tetra != null)
            {
                target = tetra.gameObject;
                mover.targetPoint = target.transform.position;
                mover.enabled = true;
                targetGrabbable = target.GetComponent<Grabbable>();
                tetra.targetedByAgent = true;
            }
            else
            {
                Debug.Log("Could not find valid target. Dying.");
                target  = null;
                mover.enabled = false;
                Die();
            }
        }

        private void Die()
        {
            this.enabled = false;
            dyingBehaviour.enabled = true;
        }
    }
}
