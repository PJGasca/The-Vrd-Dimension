using Assets.Scripts.Game;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(AccelerateTowardsPoint))]
    [RequireComponent(typeof(ChaosAgentDying))]
    public class ChaosAgentSeeking : MonoBehaviour
    {
        private Rigidbody rb;
        private AccelerateTowardsPoint mover;
        public GameObject target = null;
        private Grabbable targetGrabbable = null;
        private ChaosAgentDying dyingBehaviour;
        private AgentScaler scaler;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mover = GetComponent<AccelerateTowardsPoint>();
            dyingBehaviour = GetComponent<ChaosAgentDying>();
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
            else
            {
                mover.targetPoint = target.transform.position;
            }
        }

        private void PickNewTarget()
        {
            Tetrahedron[] tetras = Tetrahedron.All;

            if (target!=null)
            {
                target.GetComponent<Tetrahedron>().targetedByAgent = false;
            }

            Tetrahedron tetra = GameManager.Instance.GetBreakableTetra();
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

        public void OnCollisionEnter(Collision collision)
        {
            Tetrahedron tetra = collision.gameObject.GetComponent<Tetrahedron>();
            if(tetra!=null && tetra.gameObject.GetComponent<ObjectSize>().Size>1)
            {
                tetra.Split();
                Die();
            }
        }
    }
}
